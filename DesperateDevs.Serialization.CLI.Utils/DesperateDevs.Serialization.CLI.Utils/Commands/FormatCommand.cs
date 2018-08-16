using DesperateDevs.CLI.Utils;

namespace DesperateDevs.Serialization.CLI.Utils {

    public class FormatCommand : AbstractPreferencesCommand {

        public override string trigger { get { return "format"; } }
        public override string description { get { return "Format the config files"; } }
        public override string group { get { return "Properties"; } }
        public override string example { get { return "format [-mini]"; } }

        public FormatCommand() : base(typeof(FormatCommand).FullName) {
        }

        protected override void run() {
            _preferences.Save(_args.HasParameter("-mini"));
        }
    }
}
