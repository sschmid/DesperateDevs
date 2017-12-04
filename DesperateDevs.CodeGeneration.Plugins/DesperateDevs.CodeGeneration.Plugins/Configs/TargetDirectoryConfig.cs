using System.Collections.Generic;
using DesperateDevs.CodeGeneration.CodeGenerator;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class TargetDirectoryConfig : AbstractConfigurableConfig {

        const string TARGET_DIRECTORY_KEY = "DesperateDevs.CodeGeneration.Plugins.TargetDirectory";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { TARGET_DIRECTORY_KEY, "Assets/Sources" }
                };
            }
        }

        public string targetDirectory {
            get { return _preferences[TARGET_DIRECTORY_KEY].ToSafeDirectory(); }
        }
    }
}
