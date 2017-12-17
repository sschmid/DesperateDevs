using System.Collections.Generic;
using System.IO;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.Builder.CLI {

    public class Version : AbstractPreferencesCommand, IConfigurable {

        public override string trigger => "version";
        public override string description => "Update the version file";
        public override string example => "builder version minor";

        public Dictionary<string, string> defaultProperties => _versionConfig.defaultProperties;

        readonly VersionConfig _versionConfig = new VersionConfig();

        public Version() : base(typeof(Version).FullName) {
        }

        public void Configure(Preferences preferences) {
            _versionConfig.Configure(preferences);
        }

        protected override void run() {
            var number = _args[0];

            var version = System.Version.Parse(File.ReadAllText(_versionConfig.versionPath));
            switch (number) {
                case "patch":
                    version = new System.Version(version.Major, version.Minor, version.Build + 1);
                    break;
                case "minor":
                    version = new System.Version(version.Major, version.Minor + 1, 0);
                    break;
                case "major":
                    version = new System.Version(version.Major + 1, 0, 0);
                    break;
            }

            File.WriteAllText(_versionConfig.versionPath, version.ToString());
        }
    }
}
