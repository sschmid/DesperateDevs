#!/usr/bin/env bash
PROJECT="DesperateDevs"
PLUGINS=(utils)
RESOURCES=.bee

source "${RESOURCES}"/desperatedevs.sh

# utils
UTILS_RSYNC_INCLUDE="${RESOURCES}"/utils/rsync_include.txt
UTILS_RSYNC_EXCLUDE="${RESOURCES}"/utils/rsync_exclude.txt
