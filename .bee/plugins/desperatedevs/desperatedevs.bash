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
  new <project-name>             add new src and test project
                                 e.g. bee desperatedevs new DesperateDevs.Xyz
  new_benchmark <project-name>   add benchmark project
                                 e.g. bee desperatedevs new_benchmark DesperateDevs.Xyz
  clean                          delete build directory and all bin and obj directories
  build                          build solution
  rebuild                        clean and build solution
  test [args]                    run unit tests
  coverage                       run unit tests and generate coverage report
  restore_unity                  copy source code and samples to all unity projects
  sync_unity_solutions           generate C# project for all unity projects
  publish                        publish nupkg to nuget.org
  publish_local                  publish nupkg locally to disk
  pack_jenny                     pack Jenny
  pack_unity                     pack projects for Unity
  generate_unity_packages        generate unity packages

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
  cp -r "$(cat .unitypath)/Managed" "${BUILD}/"

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
  local coverage_dir="${BUILD}/coverage"
  rm -rf "${coverage_dir}"
  find src -type d -name TestResults -exec rm -rf {} +
  dotnet test --collect:"XPlat Code Coverage" || true
  reportgenerator "-Title:${BEE_PROJECT}" "-reports:src/**/coverage.cobertura.xml" "-targetDir:${coverage_dir}"
  find src -type d -name TestResults -exec rm -rf {} +
  echo "To see the test coverage results, please open ${coverage_dir}/index.html"
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
      _sync "src/${project}/src/bin/Release/${project}.dll" "${project_path}"
    done

    bee::log_echo "Restore Dotfiles: ${unity_project_path}"
    mkdir -p "${unity_project_path}/.dotsettings"
    cp DesperateDevs.sln.DotSettings "${unity_project_path}/$(basename "${unity_project_path}").sln.DotSettings"
    cp .dotsettings/CodeStyle.DotSettings "${unity_project_path}/.dotsettings"
    cp .dotsettings/PatternsAndTemplates.DotSettings "${unity_project_path}/.dotsettings"
    cp .dotsettings/InspectionSettings.DotSettings "${unity_project_path}/.dotsettings"
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
  desperatedevs::build
  local project_dir="${BUILD_SRC}/Jenny"
  local jenny_dir="${project_dir}/Jenny"
  local plugins_dir="${jenny_dir}/Plugins/Jenny"
  _clean_dir "${project_dir}" "${jenny_dir}" "${plugins_dir}"

  local -a projects=(
    Jenny.Generator.Cli
  )
  local -a plugins=(
    Jenny.Plugins
    Jenny.Plugins.Roslyn
    Jenny.Plugins.Unity
  )
  local -a exclude
  mapfile -t exclude < <(for project in "${projects[@]}"; do
    _get_project_references "src/${project}/src/${project}.csproj" ".dll"
  done | sort -u && cat "${BEE_RESOURCES}/desperatedevs/rsync_exclude.txt")

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/" "${jenny_dir}"; done
  for p in "${plugins[@]}"; do
    rsync \
      --archive \
      --recursive \
      --prune-empty-dirs \
      --exclude-from <(echo "${exclude[*]}") \
      "src/${p}/src/bin/Release/" "${plugins_dir}"
  done

  ln -s "Jenny.Generator.Cli" "${jenny_dir}/Jenny"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Server "${project_dir}"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Server.bat "${project_dir}"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Auto-Import "${project_dir}"
  cp src/Jenny.Generator.Cli/scripts/Jenny-Auto-Import.bat "${project_dir}"
}

desperatedevs::pack_unity() {
  desperatedevs::build
  local project_dir editor_dir jenny_dir images_dir
  local -a projects to_editor to_plugins images

  ##############################################################################
  # Desperate Devs
  ##############################################################################
  project_dir="${BUILD_SRC}/Unity/Assets/DesperateDevs"
  editor_dir="${project_dir}/Editor"
  _clean_dir "${project_dir}" "${editor_dir}"

  projects=(
    DesperateDevs.Caching
    DesperateDevs.Extensions
    DesperateDevs.Reflection
    DesperateDevs.Serialization
    DesperateDevs.Threading
    DesperateDevs.Unity
    DesperateDevs.Unity.Editor
  )
  to_editor=(
    DesperateDevs.Unity.Editor
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/${p}.dll" "${project_dir}"; done
  for f in "${to_editor[@]}"; do mv "${project_dir}/${f}.dll" "${editor_dir}"; done

  ##############################################################################
  # Jenny
  ##############################################################################
  project_dir="${BUILD_SRC}/Unity/Assets/Jenny"
  editor_dir="${project_dir}/Editor"
  jenny_dir="${editor_dir}/Jenny"
  images_dir="${editor_dir}/Images"
  _clean_dir "${project_dir}" "${editor_dir}" "${jenny_dir}" "${images_dir}"

  projects=(
    # editor
    Jenny
    Jenny.Generator
    Jenny.Generator.Unity.Editor

    # plugins
    Jenny.Plugins
    Jenny.Plugins.Unity
  )
  to_editor=(
    Jenny
    Jenny.Generator
    Jenny.Generator.Unity.Editor
  )
  to_plugins=(
    Jenny.Plugins
    Jenny.Plugins.Unity
  )
  images=(
    Jenny.Generator.Unity.Editor
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/${p}.dll" "${project_dir}"; done
  for f in "${to_editor[@]}"; do mv "${project_dir}/${f}.dll" "${editor_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${project_dir}/${f}.dll" "${jenny_dir}"; done
  for f in "${images[@]}"; do _sync "src/${f}/src/Images/" "${images_dir}"; done

  ##############################################################################
  # Sherlog
  ##############################################################################
  project_dir="${BUILD_SRC}/Unity/Assets/Sherlog"
  _clean_dir "${project_dir}"

  projects=(
    Sherlog
    Sherlog.Appenders
    Sherlog.Formatters
  )
  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/${p}.dll" "${project_dir}"; done

  ##############################################################################
  # TCPeasy
  ##############################################################################

  project_dir="${BUILD_SRC}/Unity/Assets/TCPeasy"
  _clean_dir "${project_dir}"

  projects=(
    TCPeasy
  )
  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/${p}.dll" "${project_dir}"; done
}

desperatedevs::generate_unity_packages() {
  local unity_project_path="${BUILD}/UnityPackages" csproj reference references platforms version
  local -a project_references
  _clean_dir "${unity_project_path}"
  git clone "${DESPERATE_DEVS_UNITY_PACKAGES_VERSION}" "${unity_project_path}"

  for project in "${!DESPERATE_DEVS_RESTORE_UNITY[@]}"; do
    bee::log_echo "Update ${project}"
    rm -f "${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}/${project}/"*.cs
    mkdir -p "${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}"
    _sync_unity "src/${project}/src/" "${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}/${project}"
    csproj="src/${project}/src/${project}.csproj"

    mapfile -t project_references < <(_get_project_references "${csproj}" | sort -u)
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
    ln -sf ../../.dotsettings/CodeStyle.DotSettings CodeStyle.DotSettings
    ln -sf ../../.dotsettings/PatternsAndTemplates.DotSettings PatternsAndTemplates.DotSettings
    ln -sf ../../.dotsettings/InspectionSettings.DotSettings InspectionSettings.DotSettings
  popd > /dev/null || exit 1

  UNITY_PROJECT_PATH="${unity_project_path}"
  version="$(grep "m_EditorVersion:" "${unity_project_path}/ProjectSettings/ProjectVersion.txt" | awk '{print $2}')"
  UNITY="${UNITY_PATH}/${version}/${UNITY_APP}"
  unity::sync_solution
}

_get_project_references() {
  local reference ext="${2:-}"
  while read -r reference; do
    reference="$(basename "${reference}" .csproj)"
    echo "${reference}${ext}"
    _get_project_references "src/${reference}/src/${reference}.csproj" "${ext}"
  done < <(dotnet list "$1" reference | tail -n +3)
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
