: "${BUILD_SRC:=build/src}"

desperatedevs::comp() {
  if ((!$# || $# == 1 && COMP_PARTIAL)); then
    bee::comp_plugin desperatedevs
  elif (($# == 1 || $# == 2 && COMP_PARTIAL)); then
    case "$1" in
      new) echo "DesperateDevs."; return ;;
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
  local name="$1"
  dotnet new classlib -n "${name}" -o "src/${name}/src"
  cat << 'EOF' > "src/${name}/src/${name}.csproj"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>$(DefaultTargetFramework)</TargetFramework>
    <PackageVersion>0.1.0</PackageVersion>
  </PropertyGroup>

</Project>
EOF
  dotnet sln add -s "${name}" "src/${name}/src/${name}.csproj"

  local test_name="${name}.Tests"
  dotnet new xunit -n "${test_name}" -o "src/${name}/tests"
  cat << 'EOF' > "src/${name}/tests/${test_name}.csproj"
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
  dotnet sln add -s "${name}" "src/${name}/tests/${test_name}.csproj"
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
  _symlink_unity_src
  _symlink_desperate_devs_unity
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
  rsync -ahriI --include-from "${DEDE_RSYNC_INCLUDE}" --exclude-from "${DEDE_RSYNC_EXCLUDE}" "$@"
}

_symlink_unity_src() {
  local source_files project_dir file
  mapfile -t source_files < <(find src/DesperateDevs.Tests/unity/src -type f -name "*.cs")
  for unity_version in "${DEDE_UNITY_VERSIONS[@]}"; do
    project_dir="src/DesperateDevs.Tests/unity/DesperateDevs-${unity_version}"
    bee::log_echo "Restore src: ${project_dir}"
    _clean_dir "${project_dir}/Assets/src"
    pushd "${project_dir}" > /dev/null || exit 1
      for f in "${source_files[@]}"; do
        file="${f##*/}"
        bee::log_echo "  Restoring ${project_dir}/Assets/src/${file}"
        ln -s "../../../src/${file}" "Assets/src/${file}"
      done
      bee::log_echo "  Restoring ${project_dir}/Jenny.properties"
      ln -sf "../src/Jenny.properties" "Jenny.properties"
    popd > /dev/null || exit 1
  done
}

_symlink_desperate_devs_unity() {
  local source_files project_dir target_dir file
  for unity_version in "${DEDE_UNITY_VERSIONS[@]}"; do
    project_dir="src/DesperateDevs.Tests/unity/DesperateDevs-${unity_version}"
    bee::log_echo "Restore DesperateDevs: ${project_dir}"
    _clean_dir "${project_dir}/Assets/DesperateDevs"
    for dep in "${DESPERATE_DEVS_RESTORE_UNITY[@]}"; do
      mapfile -t source_files < <(find "src/${dep}/src" -type f -name "*.cs" -not -path "*src/obj*")
      bee::log_echo "Restore ${dep}: ${project_dir}"
      mkdir -p "${project_dir}/Assets/DesperateDevs/${dep}"
      pushd "${project_dir}/Assets/DesperateDevs/${dep}" > /dev/null || exit 1
        for f in "${source_files[@]}"; do
          file="${f##*/}"
          bee::log_echo "  Restoring ${project_dir}/Assets/DesperateDevs/${dep}/${file}"
          ln -s "../../../../../../../${f}" "${file}"
        done
      popd > /dev/null || exit 1
    done
    pushd "${project_dir}/Assets/DesperateDevs" > /dev/null || exit 1
      for dir in "${DESPERATE_DEVS_DIRS[@]}"; do
        bee::log_echo "Restore ${dir}: ${project_dir}"
        target_dir="$(dirname "${dir}")"
        mkdir -p "${target_dir}"
        pushd "${target_dir}" > /dev/null || exit 1
          bee::log_echo "  Restoring ${target_dir}"
          ln -s "../../../../../../../${dir}" "$(basename "${dir}")"
        popd > /dev/null || exit 1
      done
    popd > /dev/null || exit 1
  done
}
