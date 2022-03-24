using DesperateDevs.CLI.Utils;

namespace DesperateDevs.Serialization.CLI.Utils
{
    public class FormatCommand : AbstractPreferencesCommand
    {
        public override string trigger => "format";
        public override string description => "Format the config files";
        public override string group => CommandGroups.PROPERTIES;
        public override string example => "format [-mini]";

        public FormatCommand() : base(typeof(FormatCommand).FullName)
        {
        }

        protected override void run()
        {
            _preferences.Minified = _args.HasParameter("-mini");
            _preferences.Save();
        }
    }
}
