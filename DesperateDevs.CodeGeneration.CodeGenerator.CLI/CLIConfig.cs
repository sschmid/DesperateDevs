using System.Collections.Generic;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public static class CLIConfigExtension {

        public static CLIConfig CreateCLIConfig(this Preferences preferences) {
            var config = new CLIConfig();
            config.Configure(preferences);
            return config;
        }
    }

    public class CLIConfig : AbstractConfigurableConfig {

        const string IGNORE_UNUSED_KEYS_KEY = "CodeGenerator.CLI.Ignore.UnusedKeys";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { IGNORE_UNUSED_KEYS_KEY, string.Empty }
                };
            }
        }

        public string[] ignoreUnusedKeys {
            get { return _preferences[IGNORE_UNUSED_KEYS_KEY].ArrayFromCSV(); }
            set { _preferences[IGNORE_UNUSED_KEYS_KEY] = value.ToCSV(); }
        }
    }
}
