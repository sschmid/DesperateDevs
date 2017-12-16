using System.Linq;

namespace DesperateDevs.Serialization.CLI.Utils {

    public class FixConfig : AbstractPreferencesCommand {

        public override string trigger { get { return "fix"; } }
        public override string description { get { return "Add missing or remove unused keys interactively"; } }
        public override string example { get { return "fix"; } }

        public FixConfig() : base(typeof(FixConfig).Name) {
        }

        protected override void run() {
            var requiredProperties = CLIUtil.GetDefaultProperties();
            var requiredKeys = requiredProperties.Keys.ToArray();
            var missingKeys = _preferences.GetMissingKeys(requiredKeys);

            foreach (var key in missingKeys) {
                _preferences.ForceAddKey("Will add missing key", key, requiredProperties[key]);
            }
        }
    }
}
