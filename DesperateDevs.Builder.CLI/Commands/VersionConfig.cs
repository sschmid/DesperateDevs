using System.Collections.Generic;
using DesperateDevs.Serialization;

namespace DesperateDevs.Builder.CLI {

    public class VersionConfig : AbstractConfigurableConfig {

        const string VERSION_PATH = "Builder.Version.Path";

        public override Dictionary<string, string> defaultProperties => new Dictionary<string, string> {
            [VERSION_PATH] = "version.txt"
        };

        public string versionPath {
            get { return _preferences[VERSION_PATH]; }
            set { _preferences[VERSION_PATH] = value; }
        }
    }
}
