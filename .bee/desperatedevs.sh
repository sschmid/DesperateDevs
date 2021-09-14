#!/usr/bin/env bash

BUILD_SRC=Build/src

desperatedevs::cover() {
  rm -rf coverage
  find src -type d -name TestResults -exec rm -rf {} +
  dotnet test --collect:"XPlat Code Coverage" || true
  reportgenerator "-Title:Desperate Devs" "-reports:src/**/coverage.cobertura.xml" "-targetDir:coverage"
  find src -type d -name TestResults -exec rm -rf {} +
  open coverage/index.html
}

desperatedevs::clear() {
  find . -type d -name obj -exec rm -rf {} +
  find . -type d -name bin -exec rm -rf {} +
}

desperatedevs::collect_jenny() {
  log_func
  local jenny_dir="${BUILD_SRC}/Jenny"
  local codegenerator_dir="${jenny_dir}/Jenny"
  local plugins_dir="${codegenerator_dir}/Plugins/DesperateDevs"
  utils::clean_dir "${jenny_dir}" "${codegenerator_dir}" "${plugins_dir}"

  local projects=(
    "DesperateDevs.CodeGeneration.CodeGenerator.CLI"
    "DesperateDevs.CodeGeneration.Plugins"
    "DesperateDevs.CodeGeneration.Unity.Plugins"
  )
  local to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )

  for p in "${projects[@]}"; do utils::sync "${p}/bin/Release/" "${codegenerator_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${codegenerator_dir}/${f}" "${plugins_dir}"; done

  mv "${codegenerator_dir}/DesperateDevs.CodeGeneration.CodeGenerator.CLI.exe" "${codegenerator_dir}/Jenny.exe"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Server" "${jenny_dir}"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Server.bat" "${jenny_dir}"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Auto-Import" "${jenny_dir}"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Auto-Import.bat" "${jenny_dir}"
}

desperatedevs::collect_jenny_unity() {
  log_func
  local codegenerator_dir="${BUILD_SRC}/Unity/Jenny/Assets/Jenny"
  local editor_dir="${codegenerator_dir}/Editor"
  local images_dir="${editor_dir}/Images"
  local plugins_dir="${editor_dir}/Plugins/DesperateDevs"
  utils::clean_dir "${codegenerator_dir}" "${editor_dir}" "${images_dir}" "${plugins_dir}"

  local projects=(
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor"
    "DesperateDevs.CodeGeneration.Plugins"
    "DesperateDevs.CodeGeneration.Unity.Plugins"
  )
  local to_editor=(
    "DesperateDevs.Analytics.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.dll"
    "DesperateDevs.CodeGeneration.dll"
    "DesperateDevs.Networking.dll"
    "DesperateDevs.Serialization.dll"
    "DesperateDevs.Unity.Editor.dll"
  )
  local images=(
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/Images/"
  )
  local to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )

  for p in "${projects[@]}"; do utils::sync "${p}/bin/Release/" "${codegenerator_dir}"; done
  for f in "${to_editor[@]}"; do mv "${codegenerator_dir}/${f}" "${editor_dir}"; done
  for f in "${images[@]}"; do utils::sync "${f}" "${images_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${codegenerator_dir}/${f}" "${plugins_dir}"; done
}

desperatedevs::collect_desperatedevs_unity() {
  log_func
  local desperatedevs_dir="${BUILD_SRC}/DesperateDevs"
  local editor_dir="${desperatedevs_dir}/Editor"
  local images_dir="${editor_dir}/Images"
  local plugins_dir="${editor_dir}/Plugins/DesperateDevs"
  utils::clean_dir "${desperatedevs_dir}" "${editor_dir}" "${images_dir}" "${plugins_dir}"

  local projects=(
    # all
    "DesperateDevs.Logging"
    "DesperateDevs.Logging.Appenders"
    "DesperateDevs.Logging.Formatters"
    "DesperateDevs.Networking"
    "DesperateDevs.Serialization"
    "DesperateDevs.Threading"
    "DesperateDevs.Threading.Promises"
    "DesperateDevs.Threading.Promises.Unity"
    "DesperateDevs.Utils"

    # editor
    "DesperateDevs.Analytics"
    "DesperateDevs.CodeGeneration"
    "DesperateDevs.CodeGeneration.CodeGenerator"
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor"
    "DesperateDevs.Unity.Editor"

    # plugins
    "DesperateDevs.CodeGeneration.Plugins"
    "DesperateDevs.CodeGeneration.Unity.Plugins"
  )
  local to_editor=(
    "DesperateDevs.Analytics.dll"
    "DesperateDevs.CodeGeneration.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.dll"
    "DesperateDevs.Unity.Editor.dll"
  )
  local images=(
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor/Images/"
  )
  local to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )

  for p in "${projects[@]}"; do utils::sync "${p}/bin/Release/" "${desperatedevs_dir}"; done
  for f in "${to_editor[@]}"; do mv "${desperatedevs_dir}/${f}" "${editor_dir}"; done
  for f in "${images[@]}"; do utils::sync "${f}" "${images_dir}"; done
  for f in "${to_plugins[@]}"; do mv "${desperatedevs_dir}/${f}" "${plugins_dir}"; done
}

desperatedevs::collect() {
  desperatedevs::collect_desperatedevs_unity
  desperatedevs::collect_jenny
  desperatedevs::collect_jenny_unity
}

desperatedevs::sync_unity_codegenerator() {
  log_func

  desperatedevs::collect_jenny
  local cli="Tests/Unity/CodeGenerator/Jenny"
  utils::clean_dir "${cli}"
  utils::sync "${BUILD_SRC}/Jenny/" "${cli}/.."

  desperatedevs::collect_jenny_unity
  local unity_libs="Tests/Unity/CodeGenerator/Assets/Libraries"
  utils::clean_dir "${unity_libs}"
  utils::sync "${BUILD_SRC}/Unity/Jenny/Assets/Jenny" "${unity_libs}"
}

desperatedevs::sync() {
  desperatedevs::sync_unity_codegenerator
}

desperatedevs::pack() {
  log_func
  desperatedevs::rebuild
  desperatedevs::run_tests
  desperatedevs::collect
}

desperatedevs::rebuild() {
  log_func
  msbuild /t:Clean /p:Configuration=Release /v:m
  msbuild -t:restore
  msbuild /p:Configuration=Release /v:m
}

desperatedevs::run_tests() {
  log_func
  msbuild /p:Configuration=Release /v:m Tests/Tests.csproj
  mono Tests/bin/Release/Tests.exe "$@"
}
