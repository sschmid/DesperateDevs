﻿using System;

namespace DesperateDevs.CodeGeneration.Plugins {

    public static class TargetDirectoryStringExtension {

        public static string ToSafeDirectory(this string directory) {
            if (string.IsNullOrEmpty(directory) || directory == ".") {
                return "Generated";
            }

            if (directory.EndsWith("/", StringComparison.Ordinal)) {
                directory = directory.Substring(0, directory.Length - 1);
            }

            if (!directory.EndsWith("/Generated", StringComparison.OrdinalIgnoreCase)) {
                directory += "/Generated";
            }

            return directory;
        }
    }
}
