using System.Collections.Generic;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class CLIConfig : AbstractConfigurableConfig {

        const string IGNORE_UNUSED_KEYS_KEY = "Jenny.Ignore.Keys";

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
