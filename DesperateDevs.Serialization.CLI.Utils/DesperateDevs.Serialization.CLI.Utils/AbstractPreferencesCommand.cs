using System;
using System.IO;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.Serialization.CLI.Utils {

    public abstract class AbstractPreferencesCommand : AbstractCommand {

        protected readonly Logger _logger;
        protected Preferences _preferences;

        protected AbstractPreferencesCommand(string loggerName) {
            _logger = fabl.GetLogger(loggerName);
        }

        public override void Run(string[] args) {
            try {
                _preferences = new Preferences(args.GetPropertiesPath(), args.GetUserPropertiesPath());
                var configurable = this as IConfigurable;
                if (configurable != null) {
                    configurable.Configure(_preferences);
                }

                var fullPath = Path.GetFullPath(_preferences.propertiesPath);
                var workingDir = Path.GetDirectoryName(fullPath);
                _logger.Debug("Setting working directory to " + workingDir);
                Directory.SetCurrentDirectory(workingDir);
            } catch (Exception ex) {
                _logger.Error(ex.Message);

                return;
            }

            base.Run(args);
        }
    }
}
