#!/bin/bash

BUILD_SRC=Build/src

desperatedevs::collect_jenny() {
  log_func
  local jenny="${BUILD_SRC}/Jenny"
  local codegenerator="${jenny}/Jenny"
  local plugins="${codegenerator}/Plugins/DesperateDevs"
  utils::clean_dir "${jenny}" "${codegenerator}" "${plugins}"

  local projects=(
    "DesperateDevs.CodeGeneration.CodeGenerator.CLI"
    "DesperateDevs.CodeGeneration.Plugins"
    "DesperateDevs.CodeGeneration.Unity.Plugins"
  )
  local to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )

  for p in "${projects[@]}"; do utils::sync "${p}/bin/Release/" "${codegenerator}"; done
  for f in "${to_plugins[@]}"; do mv "${codegenerator}/${f}" "${plugins}"; done

  mv "${codegenerator}/DesperateDevs.CodeGeneration.CodeGenerator.CLI.exe" "${codegenerator}/Jenny.exe"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Server" "${jenny}"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Server.bat" "${jenny}"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Auto-Import" "${jenny}"
  cp "DesperateDevs.CodeGeneration.CodeGenerator.CLI/Jenny-Auto-Import.bat" "${jenny}"
}

desperatedevs::collect_jenny_unity() {
  log_func
  local codegenerator="${BUILD_SRC}/Unity/Jenny/Assets/Jenny"
  local editor="${codegenerator}/Editor"
  local plugins="${editor}/Plugins/DesperateDevs"
  utils::clean_dir "${codegenerator}" "${editor}" "${plugins}"

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
  local to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )

  for p in "${projects[@]}"; do utils::sync "${p}/bin/Release/" "${codegenerator}"; done
  for f in "${to_editor[@]}"; do mv "${codegenerator}/${f}" "${editor}"; done
  for f in "${to_plugins[@]}"; do mv "${codegenerator}/${f}" "${plugins}"; done
}

desperatedevs::collect_desperatedevs_unity() {
  log_func
  local desperatedevs="${BUILD_SRC}/DesperateDevs"
  local editor="${desperatedevs}/Editor"
  local plugins="${editor}/Plugins/DesperateDevs"
  utils::clean_dir "${desperatedevs}" "${editor}" "${plugins}"

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
  local to_plugins=(
    "DesperateDevs.CodeGeneration.Plugins.dll"
    "DesperateDevs.CodeGeneration.Unity.Plugins.dll"
  )

  for p in "${projects[@]}"; do utils::sync "${p}/bin/Release/" "${desperatedevs}"; done
  for f in "${to_editor[@]}"; do mv "${desperatedevs}/${f}" "${editor}"; done
  for f in "${to_plugins[@]}"; do mv "${desperatedevs}/${f}" "${plugins}"; done
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
  utils::sync "${BUILD_SRC}/Jenny/" "${cli}"

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
  dotnet::clean_build
  dotnet::tests
  desperatedevs::collect
}
