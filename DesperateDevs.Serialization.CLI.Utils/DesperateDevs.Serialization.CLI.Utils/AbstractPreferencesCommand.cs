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
                var propertiesPath = args.GetPropertiesPath();
                if (propertiesPath == null) {
                    var allPreferences = Preferences.FindAll("*.properties");
                    if (allPreferences.Length == 0) {
                        _logger.Warn("Couldn't find any *.properties files!");
                        _logger.Info("Use 'new Preferences.properties' to create an new file.");
                        return;
                    }
                } else if (!File.Exists(propertiesPath)) {
                    throw new Exception("The file " + propertiesPath + " does not exist.");
                }

                var userPropertiesPath = args.GetUserPropertiesPath();
                if (userPropertiesPath != null && !File.Exists(userPropertiesPath)) {
                    throw new Exception("The file " + userPropertiesPath + " does not exist.");
                }

                _preferences = new Preferences(propertiesPath, userPropertiesPath);
                var configurable = this as IConfigurable;
                if (configurable != null) {
                    configurable.Configure(_preferences);
                }

            } catch (Exception ex) {
                _logger.Error(ex.Message);

                return;
            }

            base.Run(args);
        }
    }
}
