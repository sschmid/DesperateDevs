using System;
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
            } catch (Exception ex) {
                _logger.Error(ex.Message);

                return;
            }

            base.Run(args);
        }
    }
}
