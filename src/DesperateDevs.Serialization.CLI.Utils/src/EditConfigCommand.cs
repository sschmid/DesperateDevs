using System.Threading;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class EditConfigCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "edit";
        public override string Description => "Open config files with default editor";
        public override string Group => CommandGroups.Properties;
        public override string Example => "edit";

        public EditConfigCommand() : base(typeof(EditConfigCommand).FullName) { }

        protected override void Run()
        {
            _logger.Debug($"Opening {_preferences.UserPropertiesPath}");
            System.Diagnostics.Process.Start(_preferences.UserPropertiesPath);

            Thread.Sleep(100);

            _logger.Debug($"Opening {_preferences.PropertiesPath}");
            System.Diagnostics.Process.Start(_preferences.PropertiesPath);
        }
    }
}
