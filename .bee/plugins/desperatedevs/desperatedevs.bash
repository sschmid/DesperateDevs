: "${BUILD:=build}"

BUILD_SRC="${BUILD}/src"

desperatedevs::help() {
  cat << 'EOF'
template:
  DESPERATE_DEVS_NUGET_LOCAL=~/.nuget/local
  DESPERATE_DEVS_UNITY_PACKAGES_VERSION=https://github.com/sschmid/Unity-2021.3.git
  DESPERATE_DEVS_UNITY_PROJECTS=()
  DESPERATE_DEVS_RESTORE_UNITY=([key]=value)

usage:
  docker                         build and run desperatedevs docker image
  new <project-name>             add new project and test project
                                 e.g. bee desperatedevs new DesperateDevs.Xyz
  new_benchmark <project-name>   add benchmark project
                                 e.g. bee desperatedevs new_benchmark DesperateDevs.Xyz
  clean                          delete build directory and all bin and obj directories
  build                          build solution
  rebuild                        clean and build solution
  test [args]                    run all tests
  coverage                       run all tests and generate coverage report
  restore_unity                  copy source code and samples to all unity projects
  sync_unity_solutions           generate C# project for all unity projects
  generate_unity_packages        generate unity packages
  publish                        publish nupkg to nuget.org
  publish_local                  publish nupkg locally to disk
  pack_jenny                     pack Jenny
  pack_jenny_unity               pack Jenny for Unity
  pack_desperatedevs_unity       pack DesperateDevs for Unity
  pack                           pack all the above

EOF
}

desperatedevs::comp() {
  if ((!$# || $# == 1 && COMP_PARTIAL)); then
    bee::comp_plugin desperatedevs
  elif (($# == 1 || $# == 2 && COMP_PARTIAL)); then
    case "$1" in
      new|new_benchmark) echo "DesperateDevs."; return ;;
    esac
  fi
}

desperatedevs::docker() {
  if [[ ! -f .unitypath ]]; then
    cat << EOF
Before running docker, please provide the full path to your Unity installation.
This is required to resolve dependencies to dlls like UnityEngine.dll or UnityEditor.dll.

Example (this may be different on your machine):

  Windows: C:/Program Files/Unity/Hub/Editor/<UNITY_VERSION>/Editor/Data
  macOS: /Applications/Unity/Hub/Editor/<UNITY_VERSION>/Unity.app/Contents
  Linux: ~/Unity/Hub/Editor/<UNITY_VERSION>/Editor/Data

EOF

    local path
    read -rp "Path to Unity dlls: " path; echo
    [[ -z "${path}" || ! -d "${path}" ]] && echo "No such directory. Abort." && exit 1
    echo "${path}" > .unitypath
  fi

  mkdir -p "${BUILD}"
  cp -r "$(cat .unitypath)/Managed" "$(pwd)/${BUILD}/"

  DOCKER_BUILDKIT=1 docker build --target bee -t desperatedevs .
  docker run -it -v "$(pwd)":/DesperateDevs -w /DesperateDevs desperatedevs "$@"
}

desperatedevs::new() {
  local name="$1" path
  path="src/${name}/src"
  dotnet new classlib -n "${name}" -o "${path}"
  cat << 'EOF' > "${path}/${name}.csproj"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>$(DefaultTargetFramework)</TargetFramework>
    <PackageVersion>0.1.0</PackageVersion>
  </PropertyGroup>

</Project>
EOF
  dotnet sln add -s "${name}" "${path}/${name}.csproj"

  local test_name="${name}.Tests" path="src/${name}/tests"
  dotnet new xunit -n "${test_name}" -o "${path}"
  cat << 'EOF' > "${path}/${test_name}.csproj"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>$(DefaultTestTargetFramework)</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsPublishable>false</IsPublishable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentAssertions" Version="6.5.1" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.1.0" />
    <PackageReference Include="Microsoft.TestPlatform.ObjectModel" Version="17.1.0" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.3" />
    <PackageReference Include="coverlet.collector" Version="3.1.2" />
  </ItemGroup>

</Project>
EOF
  dotnet sln add -s "${name}" "${path}/${test_name}.csproj"
}

desperatedevs::new_benchmark() {
  local name="$1"
  local benchmark_name="${name}.Benchmarks" path="src/${name}/benchmarks"
  dotnet new console -n "${benchmark_name}" -o "${path}"
  cat << 'EOF' > "${path}/${benchmark_name}.csproj"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>$(DefaultNetTargetFramework)</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsPublishable>false</IsPublishable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" Version="0.13.1" />
  </ItemGroup>

</Project>
EOF
  dotnet sln add -s "${name}" "${path}/${benchmark_name}.csproj"
}

desperatedevs::clean() {
  find . -type d -name obj -exec rm -rf {} +
  find . -type d -name bin -exec rm -rf {} +
  rm -rf "${BUILD}"
}

desperatedevs::build() {
  dotnet build -c Release
}

desperatedevs::rebuild() {
  desperatedevs::clean
  dotnet build -c Release
}

desperatedevs::test() {
  dotnet test -c Release "$@"
}

desperatedevs::coverage() {
  rm -rf coverage
  find src -type d -name TestResults -exec rm -rf {} +
  dotnet test --collect:"XPlat Code Coverage" || true
  reportgenerator "-Title:${BEE_PROJECT}" "-reports:src/**/coverage.cobertura.xml" "-targetDir:coverage"
  find src -type d -name TestResults -exec rm -rf {} +
  echo "To see the test coverage results, please open coverage/index.html"
}

desperatedevs::restore_unity() {
  desperatedevs::rebuild
  local project_path
  for unity_project_path in "${DESPERATE_DEVS_UNITY_PROJECTS[@]}"; do
    bee::log_echo "Restore Samples: ${unity_project_path}"
    _clean_dir "${unity_project_path}/Assets" "${unity_project_path}/Assets/Samples"
    _sync_unity src/DesperateDevs.Tests/unity/Samples "${unity_project_path}/Assets"
    mv "${unity_project_path}/Assets/Samples/Jenny.properties" "${unity_project_path}/Jenny.properties"
    mv "${unity_project_path}/Assets/Samples/Sample.properties" "${unity_project_path}/Sample.properties"

    bee::log_echo "Restore DesperateDevs: ${unity_project_path}"

    for project in "${!DESPERATE_DEVS_RESTORE_UNITY[@]}"; do
      bee::log_echo "Restore ${project}: ${unity_project_path}"
      project_path="${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}"
      mkdir -p "${project_path}"

      # sources
      # _sync_unity "src/${project}/src/" "${project_path}/${project}"

      # dlls
      _sync "src/${project}/src/bin/Release/${project}.dll" "${project_path}/${project}.dll"
    done

    bee::log_echo "Restore Dotfiles: ${unity_project_path}"
    cp DesperateDevs.sln.DotSettings "${unity_project_path}/$(basename "${unity_project_path}").sln.DotSettings"
    cp CodeStyle.DotSettings "${unity_project_path}/CodeStyle.DotSettings"
    cp PatternsAndTemplates.DotSettings "${unity_project_path}/PatternsAndTemplates.DotSettings"
  done
}

desperatedevs::sync_unity_solutions() {
  local version
  local -A projects_pids=()
  for unity_project_path in "${DESPERATE_DEVS_UNITY_PROJECTS[@]}" ; do
    UNITY_PROJECT_PATH="${unity_project_path}"
    version="$(grep "m_EditorVersion:" "${unity_project_path}/ProjectSettings/ProjectVersion.txt" | awk '{print $2}')"
    UNITY="${UNITY_PATH}/${version}/${UNITY_APP}"
    unity::sync_solution &
    projects_pids["${unity_project_path}"]=$!
  done

  for unity_project_path in "${!projects_pids[@]}"; do
    if wait ${projects_pids["${unity_project_path}"]}
    then projects_pids["${unity_project_path}"]=1
    else projects_pids["${unity_project_path}"]=0
    fi
  done

  for project in "${!projects_pids[@]}"; do
    if ((projects_pids["${project}"]))
    then bee::log_echo "🟢 ${project}"
    else bee::log_echo "🔴 ${project}"
    fi
  done | LC_ALL=C sort
}

desperatedevs::generate_unity_packages() {
  local unity_project_path="${BUILD}/UnityPackages" csproj project_references reference references platforms version
  _clean_dir "${unity_project_path}"
  git clone "${DESPERATE_DEVS_UNITY_PACKAGES_VERSION}" "${unity_project_path}"

  for project in "${!DESPERATE_DEVS_RESTORE_UNITY[@]}"; do
    bee::log_echo "Update ${project}"
    rm -f "${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}/${project}/"*.cs
    mkdir -p "${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}"
    _sync_unity "src/${project}/src/" "${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}/${project}"
    csproj="src/${project}/src/${project}.csproj"

    mapfile -t project_references < <(get_project_references "${csproj}" | sort -u)
    references=""
    for reference in "${project_references[@]}"; do
      references+=", \"${reference}\""
    done

    if [[ ${DESPERATE_DEVS_RESTORE_UNITY["${project}"]} == "Assets/Editor" ]]
    then platforms="\"Editor\""
    else platforms=""
    fi

    cat << EOF > "${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}/${project}/${project}.asmdef"
{
  "name": "${project}",
  "rootNamespace": "",
  "references": [${references:2}],
  "includePlatforms": [${platforms}],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
EOF
  done

  pushd "${unity_project_path}" > /dev/null || exit 1
    ln -sf ../../DesperateDevs.sln.DotSettings UnityPackages.sln.DotSettings
    ln -sf ../../CodeStyle.DotSettings CodeStyle.DotSettings
    ln -sf ../../PatternsAndTemplates.DotSettings PatternsAndTemplates.DotSettings
  popd > /dev/null || exit 1

  UNITY_PROJECT_PATH="${unity_project_path}"
  version="$(grep "m_EditorVersion:" "${unity_project_path}/ProjectSettings/ProjectVersion.txt" | awk '{print $2}')"
  UNITY="${UNITY_PATH}/${version}/${UNITY_APP}"
  unity::sync_solution
}

get_project_references() {
  local reference
  while read -r reference; do
    reference="$(basename "${reference}" .csproj)"
    echo "${reference}"
    get_project_references "src/${reference}/src/${reference}.csproj"
  done < <(dotnet list "$1" reference | tail -n +3)
}

desperatedevs::publish() {
  desperatedevs::clean
  dotnet pack -c Release
  dotnet nuget push "**/*.nupkg" \
      --api-key "${NUGET_API_KEY}" \
      --skip-duplicate \
      --source https://api.nuget.org/v3/index.json
}

desperatedevs::publish_local() {
  desperatedevs::clean
  dotnet pack -c Release
  _clean_dir "${DESPERATE_DEVS_NUGET_LOCAL}"
  find . -type f -name "*.nupkg" -exec nuget add {} -Source "${DESPERATE_DEVS_NUGET_LOCAL}" \;
}

desperatedevs::pack_jenny() {
  bee::log_func
  local jenny_dir="${BUILD_SRC}/Jenny"
  local code_generator_dir="${jenny_dir}/Jenny"
  local plugins_dir="${code_generator_dir}/Plugins/DesperateDevs"
  _clean_dir "${jenny_dir}" "${code_generator_dir}" "${plugins_dir}"

  local projects=(
    Jenny.Generator.Cli
    Jenny.Plugins
    Jenny.Plugins.Roslyn
    Jenny.Plugins.Unity
  )
  local to_plugins=(
    Jenny.Plugins
    Jenny.Plugins.Roslyn
    Jenny.Plugins.Unity
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/" "${code_generator_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${code_generator_dir}/${f}.dll" "${plugins_dir}"; done

  ln -s "Jenny.Generator.Cli" "${code_generator_dir}/Jenny"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Server "${jenny_dir}"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Server.bat "${jenny_dir}"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Auto-Import "${jenny_dir}"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Auto-Import.bat "${jenny_dir}"
}

desperatedevs::pack_jenny_unity() {
  bee::log_func
  local code_generator_dir="${BUILD_SRC}/Unity/Jenny/Assets/Jenny"
  local editor_dir="${code_generator_dir}/Editor"
  local images_dir="${editor_dir}/Images"
  local plugins_dir="${editor_dir}/Plugins/DesperateDevs"
  _clean_dir "${code_generator_dir}" "${editor_dir}" "${images_dir}" "${plugins_dir}"

  local projects=(
    Jenny.Generator.Unity.Editor
    Jenny.Plugins
    Jenny.Plugins.Unity
  )
  local to_editor=(
    DesperateDevs.Unity.Editor
    Jenny.Generator.Unity.Editor
    Jenny.Generator
    Jenny
    TCPeasy
  )
  local images=(
    Jenny.Generator.Unity.Editor
  )
  local to_plugins=(
    Jenny.Plugins
    Jenny.Plugins.Unity
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/"*.dll "${code_generator_dir}"; done
  for f in "${to_editor[@]}"; do mv "${code_generator_dir}/${f}.dll" "${editor_dir}"; done
  for f in "${images[@]}"; do _sync "src/${f}/src/Images/" "${images_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${code_generator_dir}/${f}.dll" "${plugins_dir}"; done
}

desperatedevs::pack_desperatedevs_unity() {
  bee::log_func
  local desperatedevs_dir="${BUILD_SRC}/DesperateDevs"
  local editor_dir="${desperatedevs_dir}/Editor"
  local images_dir="${editor_dir}/Images"
  local plugins_dir="${editor_dir}/Jenny"
  _clean_dir "${desperatedevs_dir}" "${editor_dir}" "${images_dir}" "${plugins_dir}"

  local projects=(
    DesperateDevs.Caching
    DesperateDevs.Extensions
    DesperateDevs.Reflection
    DesperateDevs.Serialization
    DesperateDevs.Threading
    DesperateDevs.Unity
    Sherlog
    Sherlog.Appenders
    Sherlog.Formatters
    TCPeasy

    # unity editor
    DesperateDevs.Unity.Editor
    Jenny
    Jenny.Generator
    Jenny.Generator.Unity.Editor

    # jenny plugins
    Jenny.Plugins
    Jenny.Plugins.Unity
  )
  local to_editor=(
    DesperateDevs.Unity.Editor
    Jenny
    Jenny.Generator
    Jenny.Generator.Unity.Editor
  )
  local to_plugins=(
    Jenny.Plugins
    Jenny.Plugins.Unity
  )
  local images=(
    Jenny.Generator.Unity.Editor
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/"*.dll "${desperatedevs_dir}"; done
  for f in "${to_editor[@]}"; do mv "${desperatedevs_dir}/${f}.dll" "${editor_dir}"; done
  for f in "${images[@]}"; do _sync "src/${f}/src/Images/" "${images_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${desperatedevs_dir}/${f}.dll" "${plugins_dir}"; done
}

desperatedevs::pack() {
  desperatedevs::pack_desperatedevs_unity
  desperatedevs::pack_jenny
  desperatedevs::pack_jenny_unity
}

_clean_dir() {
  rm -rf "$@"
  mkdir -p "$@"
}

_sync() {
  rsync \
    --archive \
    --recursive \
    --prune-empty-dirs \
    --include-from "${BEE_RESOURCES}/desperatedevs/rsync_include.txt" \
    --exclude-from "${BEE_RESOURCES}/desperatedevs/rsync_exclude.txt" \
    "$@"
}

_sync_unity() {
  rsync \
    --archive \
    --recursive \
    --prune-empty-dirs \
    --exclude-from "${BEE_RESOURCES}/desperatedevs/rsync_exclude_unity.txt" \
    "$@"
}
