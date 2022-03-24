using System;
using System.Collections.Generic;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class TargetDirectoryConfig : AbstractConfigurableConfig {

        const string TARGET_DIRECTORY_KEY = "DesperateDevs.CodeGeneration.Plugins.TargetDirectory";

        public override Dictionary<string, string> DefaultProperties {
            get {
                return new Dictionary<string, string> {
                    { TARGET_DIRECTORY_KEY, "Assets" }
                };
            }
        }

        public string targetDirectory { get { return _preferences[TARGET_DIRECTORY_KEY].ToSafeDirectory(); } }
    }

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
