#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

[[ -f ~/.bee/desperatedevs.bash ]] && source ~/.bee/desperatedevs.bash

UNITY_PATH="/Applications/Unity/Hub/Editor"
UNITY_APP="Unity.app/Contents/MacOS/Unity"

#UNITY_SERIAL=""
#UNITY_USER=""
#UNITY_PASSWORD=""

UNITY_PROJECTS=(samples/Unity)

sync_unity_solution() {
  "${UNITY}" \
    -projectPath "${UNITY_PROJECT_PATH}" \
    -batchmode \
    -nographics \
    -logfile \
    -serial "${UNITY_SERIAL}" -username "${UNITY_USER}" -password "${UNITY_PASSWORD}" \
    -quit \
    -executeMethod UnityEditor.SyncVS.SyncSolution
}

declare -A projects_pids=()
for unity_project_path in "${UNITY_PROJECTS[@]}" ; do
  UNITY_PROJECT_PATH="${unity_project_path}"
  version="$(grep "m_EditorVersion:" "${unity_project_path}/ProjectSettings/ProjectVersion.txt" | awk '{print $2}')"
  UNITY="${UNITY_PATH}/${version}/${UNITY_APP}"
  sync_unity_solution &
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
  then echo "🟢 ${project}"
  else echo "🔴 ${project}"
  fi
done | LC_ALL=C sort
