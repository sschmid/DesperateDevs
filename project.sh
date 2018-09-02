#!/bin/bash

BUILD="${ROOT}/Build"
BUILD_SRC="${BUILD}/src"

# utils
UTILS_RSYNC_INCLUDE="${ROOT}/Scripts/rsync_include.txt"
UTILS_RSYNC_EXCLUDE="${ROOT}/Scripts/rsync_exclude.txt"

# dotnet
DOTNET_PROJECT_NAME="DesperateDevs"
DOTNET_SOLUTION="${ROOT}/${DOTNET_PROJECT_NAME}.sln"
DOTNET_TESTS_PROJECT="${ROOT}/Tests/Tests.csproj"
DOTNET_TESTS_RUNNER="${ROOT}/Tests/bin/Release/Tests.exe"

desperatedevs::collect_codegenerator_cli() {
  bee::log_func
  local jenny="${BUILD_SRC}/Jenny"
  local codegenerator="${jenny}/Jenny"
  local plugins="${codegenerator}/Plugins/DesperateDevs"
  utils::clean_dir "${jenny}" "${codegenerator}" "${plugins}"

  declare -a -r projects=(
    "DesperateDevs.CodeGeneration.CodeGenerator.CLI"
    "DesperateDevs.CodeGeneration.Plugins"
    "DesperateDevs.CodeGeneration.Unity.Plugins"
  )
  for p in "${projects[@]}"; do
    utils::sync_files "${ROOT}/${p}/bin/Release/" "${codegenerator}"
  done

  declare -a -r to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )
  for f in "${to_plugins[@]}"; do
    mv "${codegenerator}/${f}" "${plugins}"
  done

  mv "${codegenerator}/DesperateDevs.CodeGeneration.CodeGenerator.CLI.exe" "${codegenerator}/Jenny.exe"

  cp "${ROOT}/DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Server" "${jenny}"
  cp "${ROOT}/DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Server.bat" "${jenny}"
  cp "${ROOT}/DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Auto-Import" "${jenny}"
  cp "${ROOT}/DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Auto-Import.bat" "${jenny}"
}

desperatedevs::collect_codegenerator_unity() {
  bee::log_func
  local codegenerator="${BUILD_SRC}/Unity/Jenny/Assets/Jenny"
  local editor="${codegenerator}/Editor"
  local plugins="${editor}/Plugins/DesperateDevs"
  utils::clean_dir "${codegenerator}" "${editor}" "${plugins}"

  declare -a -r projects=(
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor"
    "DesperateDevs.CodeGeneration.Plugins"
    "DesperateDevs.CodeGeneration.Unity.Plugins"
  )
  for p in "${projects[@]}"; do
    utils::sync_files "${ROOT}/${p}/bin/Release/" "${codegenerator}"
  done

  declare -a -r to_editor=(
    "DesperateDevs.Analytics.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.dll"
    "DesperateDevs.CodeGeneration.dll"
    "DesperateDevs.Networking.dll"
    "DesperateDevs.Serialization.dll"
    "DesperateDevs.Unity.Editor.dll"
  )
  for f in "${to_editor[@]}"; do
    mv "${codegenerator}/${f}" "${editor}"
  done

  declare -a -r to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )
  for f in "${to_plugins[@]}"; do
    mv "${codegenerator}/${f}" "${plugins}"
  done
}

desperatedevs::collect_desperatedevs() {
  bee::log_func
  local desperatedevs="${BUILD_SRC}/DesperateDevs"
  local editor="${desperatedevs}/Editor"
  local plugins="${editor}/Plugins/DesperateDevs"
  utils::clean_dir "${desperatedevs}" "${editor}" "${plugins}"

  declare -a -r projects=(
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
  for p in "${projects[@]}"; do
    utils::sync_files "${ROOT}/${p}/bin/Release/" "${desperatedevs}"
  done

  declare -a -r to_editor=(
    "DesperateDevs.Analytics.dll"
    "DesperateDevs.CodeGeneration.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.dll"
    "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.dll"
    "DesperateDevs.Unity.Editor.dll"
  )
  for f in "${to_editor[@]}"; do
    mv "${desperatedevs}/${f}" "${editor}"
  done

  declare -a -r to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )
  for f in "${to_plugins[@]}"; do
    mv "${desperatedevs}/${f}" "${plugins}"
  done
}

desperatedevs::collect() {
  desperatedevs::collect_desperatedevs
  desperatedevs::collect_codegenerator_cli
  desperatedevs::collect_codegenerator_unity
}

desperatedevs::sync_unity_codegenerator() {
  bee::log_func

  desperatedevs::collect_codegenerator_cli
  local cli="${ROOT}/Tests/Unity/CodeGenerator/Jenny"
  utils::clean_dir "${cli}"
  utils::sync_files "${BUILD_SRC}/Jenny/" "${cli}"

  desperatedevs::collect_codegenerator_unity
  local unity_libs="${ROOT}/Tests/Unity/CodeGenerator/Assets/Libraries"
  utils::clean_dir "${unity_libs}"
  utils::sync_files "${BUILD_SRC}/Unity/Jenny/Assets/Jenny" "${unity_libs}"
}

desperatedevs::sync() {
  desperatedevs::sync_unity_codegenerator
}

desperatedevs::pack() {
  bee::log_func
  dotnet::clean_build
  dotnet::tests
  desperatedevs::collect
}
