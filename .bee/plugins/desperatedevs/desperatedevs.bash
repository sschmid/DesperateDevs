: "${BUILD:=build}"

desperatedevs::help() {
  cat << 'EOF'
template:
  DESPERATE_DEVS_UNITY_PROJECTS=()
  DESPERATE_DEVS_RESTORE_UNITY=([key]=value)

usage:
  docker                         build and run desperatedevs docker image
  restore_unity                  copy source code and samples to all unity projects
  sync_unity_solutions           generate C# project for all unity projects

EOF
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

desperatedevs::restore_unity() {
  dotnet publish -c Release
  local project_path file
  for unity_project_path in "${DESPERATE_DEVS_UNITY_PROJECTS[@]}"; do
    bee::log_echo "Restore DesperateDevs: ${unity_project_path}"
    for project in "${!DESPERATE_DEVS_RESTORE_UNITY[@]}"; do
      bee::log_echo "Restore ${project}: ${unity_project_path}"
      project_path="${unity_project_path}/${DESPERATE_DEVS_RESTORE_UNITY["${project}"]}"
      mkdir -p "${project_path}"
      rsync \
        --archive \
        --recursive \
        --prune-empty-dirs \
        --include-from "${BEE_RESOURCES}/desperatedevs/rsync_include.txt" \
        --exclude-from "${BEE_RESOURCES}/desperatedevs/rsync_exclude.txt" \
        "src/${project}/bin/Release/publish/"*.dll "${project_path}"
    done

    pushd "${unity_project_path}/Assets/Plugins/Editor" > /dev/null || exit 1
      while read -r file; do
        [[ ! -f ".${file}" ]] || rm "${file}"
      done < <(find . -type f -name "*.dll")
    popd > /dev/null || exit 1

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
