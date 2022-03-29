: "${BUILD_SRC:=build/src}"

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
    mkdir -p build
    cp -r "$(cat .unitypath)/Managed" "$(pwd)/build/"
  fi

  DOCKER_BUILDKIT=1 docker build --target bee -t desperatedevs .
  docker run -it -v "$(pwd)":/app -w /app desperatedevs "$@"
}

desperatedevs::new() {
  local name="$1" path="src/${name}/src"
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
    <PackageVersion>0.1.0</PackageVersion>
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
}

desperatedevs::rebuild() {
  desperatedevs::clean
  dotnet build -c Release
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
  for project in "${DEDE_UNITY_PROJECTS[@]}"; do
    bee::log_echo "Restore Samples: ${project}"
    _clean_dir "${project}/Assets/Samples"
    _sync_unity src/DesperateDevs.Tests/unity/Samples "${project}/Assets"
    mv "${project}/Assets/Samples/Jenny.properties" "${project}/Jenny.properties"

    bee::log_echo "Restore DesperateDevs: ${project}"
    rm -rf "${project}"/Assets/DesperateDevs.*
    for dep in "${DESPERATE_DEVS_RESTORE_UNITY[@]}"; do
      bee::log_echo "Restore ${dep}: ${project}"
      _sync_unity "src/${dep}/src/" "${project}/Assets/${dep}"
    done

    bee::log_echo "Restore Dotfiles: ${project}"
    cp DesperateDevs.sln.DotSettings "${project}/$(basename "${project}").sln.DotSettings"
    cp CodeStyle.DotSettings "${project}/CodeStyle.DotSettings"
    cp PatternsAndTemplates.DotSettings "${project}/PatternsAndTemplates.DotSettings"
  done
}

desperatedevs::sync_unity_solutions() {
  local version
  local -A projects=()
  for project in "${DEDE_UNITY_PROJECTS[@]}" ; do
    UNITY_PROJECT_PATH="${project}"
    version="$(grep "m_EditorVersion:" "${project}/ProjectSettings/ProjectVersion.txt" | awk '{print $2}')"
    UNITY="${UNITY_PATH}/${version}/${UNITY_APP}"
    unity::sync_solution &
    projects["${project}"]=$!
  done

  for project in "${!projects[@]}"; do
    if wait ${projects["${project}"]}
    then projects["${project}"]=1
    else projects["${project}"]=0
    fi
  done

  for project in "${!projects[@]}"; do
    if ((projects["${project}"]))
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

desperatedevs::collect_jenny() {
  bee::log_func
  local jenny_dir="${BUILD_SRC}/Jenny"
  local code_generator_dir="${jenny_dir}/Jenny"
  local plugins_dir="${code_generator_dir}/Plugins/DesperateDevs"
  _clean_dir "${jenny_dir}" "${code_generator_dir}" "${plugins_dir}"

  local projects=(
    DesperateDevs.CodeGeneration.CodeGenerator.CLI
    DesperateDevs.CodeGeneration.Plugins
    DesperateDevs.CodeGeneration.Unity.Plugins
  )
  local to_plugins=(
    DesperateDevs.CodeGeneration.Plugins
    DesperateDevs.CodeGeneration.Unity.Plugins
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/" "${code_generator_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${code_generator_dir}/${f}.dll" "${plugins_dir}"; done

  mv "${code_generator_dir}/DesperateDevs.CodeGeneration.CodeGenerator.CLI.exe" "${code_generator_dir}/Jenny.exe"

  cp src/DesperateDevs.CodeGeneration.CodeGenerator.CLI/scripts/Jenny-Server "${jenny_dir}"
  cp src/DesperateDevs.CodeGeneration.CodeGenerator.CLI/scripts/Jenny-Server.bat "${jenny_dir}"
  cp src/DesperateDevs.CodeGeneration.CodeGenerator.CLI/scripts/Jenny-Auto-Import "${jenny_dir}"
  cp src/DesperateDevs.CodeGeneration.CodeGenerator.CLI/scripts/Jenny-Auto-Import.bat "${jenny_dir}"
}

desperatedevs::collect_jenny_unity() {
  bee::log_func
  local code_generator_dir="${BUILD_SRC}/Unity/Jenny/Assets/Jenny"
  local editor_dir="${code_generator_dir}/Editor"
  local images_dir="${editor_dir}/Images"
  local plugins_dir="${editor_dir}/Plugins/DesperateDevs"
  _clean_dir "${code_generator_dir}" "${editor_dir}" "${images_dir}" "${plugins_dir}"

  local projects=(
    DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
    DesperateDevs.CodeGeneration.Plugins
    DesperateDevs.CodeGeneration.Unity.Plugins
  )
  local to_editor=(
    DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
    DesperateDevs.CodeGeneration.CodeGenerator
    DesperateDevs.CodeGeneration
    DesperateDevs.Networking
    DesperateDevs.Unity.Editor
  )
  local images=(
    DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
  )
  local to_plugins=(
    DesperateDevs.CodeGeneration.Plugins
    DesperateDevs.CodeGeneration.Unity.Plugins
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/"*.dll "${code_generator_dir}"; done
  for f in "${to_editor[@]}"; do mv "${code_generator_dir}/${f}.dll" "${editor_dir}"; done
  for f in "${images[@]}"; do _sync "src/${f}/src/Images/" "${images_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${code_generator_dir}/${f}.dll" "${plugins_dir}"; done
}

desperatedevs::collect_desperatedevs_unity() {
  bee::log_func
  local desperatedevs_dir="${BUILD_SRC}/DesperateDevs"
  local editor_dir="${desperatedevs_dir}/Editor"
  local images_dir="${editor_dir}/Images"
  local plugins_dir="${editor_dir}/Plugins/DesperateDevs"
  _clean_dir "${desperatedevs_dir}" "${editor_dir}" "${images_dir}" "${plugins_dir}"

  local projects=(
    # all
    DesperateDevs.Caching
    DesperateDevs.Extensions
    DesperateDevs.Logging
    DesperateDevs.Logging.Appenders
    DesperateDevs.Logging.Formatters
    DesperateDevs.Networking
    DesperateDevs.Reflection
    DesperateDevs.Serialization
    DesperateDevs.Threading
    DesperateDevs.Unity

    # editor
    DesperateDevs.CodeGeneration
    DesperateDevs.CodeGeneration.CodeGenerator
    DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
    DesperateDevs.Unity.Editor

    # plugins
    DesperateDevs.CodeGeneration.Plugins
    DesperateDevs.CodeGeneration.Unity.Plugins
  )
  local to_editor=(
    DesperateDevs.CodeGeneration
    DesperateDevs.CodeGeneration.CodeGenerator
    DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
    DesperateDevs.Unity.Editor
  )
  local images=(
    DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
  )
  local to_plugins=(
    DesperateDevs.CodeGeneration.Plugins
    DesperateDevs.CodeGeneration.Unity.Plugins
  )

  for p in "${projects[@]}"; do _sync "src/${p}/src/bin/Release/"*.dll "${desperatedevs_dir}"; done
  for f in "${to_editor[@]}"; do mv "${desperatedevs_dir}/${f}.dll" "${editor_dir}"; done
  for f in "${images[@]}"; do _sync "src/${f}/src/Images/" "${images_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${desperatedevs_dir}/${f}.dll" "${plugins_dir}"; done
}

desperatedevs::collect() {
  desperatedevs::collect_desperatedevs_unity
  desperatedevs::collect_jenny
  desperatedevs::collect_jenny_unity
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
