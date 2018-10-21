#!/usr/bin/env bash
PROJECT="DesperateDevs"
PLUGINS=(dotnet utils)
RESOURCES=.bee

source "${RESOURCES}"/desperatedevs.sh

# dotnet
DOTNET_SOLUTION="${PROJECT}.sln"
DOTNET_TESTS_PROJECT=Tests/Tests.csproj
DOTNET_TESTS_RUNNER=Tests/bin/Release/Tests.exe

# utils
UTILS_RSYNC_INCLUDE="${RESOURCES}"/utils/rsync_include.txt
UTILS_RSYNC_EXCLUDE="${RESOURCES}"/utils/rsync_exclude.txt
