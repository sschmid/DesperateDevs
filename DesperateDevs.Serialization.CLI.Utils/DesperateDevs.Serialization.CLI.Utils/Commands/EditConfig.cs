using System.Threading;

namespace DesperateDevs.Serialization.CLI.Utils {

    public class EditConfig : AbstractPreferencesCommand {

        public override string trigger { get { return "edit"; } }
        public override string description { get { return "Open config files with default editor"; } }
        public override string example { get { return "edit"; } }

        public EditConfig() : base(typeof(EditConfig).FullName) {
        }

        protected override void run() {
            _logger.Debug("Opening " + _preferences.userPropertiesPath);
            System.Diagnostics.Process.Start(_preferences.userPropertiesPath);

            Thread.Sleep(100);

            _logger.Debug("Opening " + _preferences.propertiesPath);
            System.Diagnostics.Process.Start(_preferences.propertiesPath);
        }
    }
}
