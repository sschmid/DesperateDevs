using DesperateDevs.Cli.Utils;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class FormatCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "format";
        public override string Description => "Format the config files";
        public override string Group => CommandGroups.Properties;
        public override string Example => "format [-mini]";

        public FormatCommand() : base(typeof(FormatCommand).FullName) { }

        protected override void Run()
        {
            _preferences.Save(_args.HasParameter("-mini"));
        }
    }
}
