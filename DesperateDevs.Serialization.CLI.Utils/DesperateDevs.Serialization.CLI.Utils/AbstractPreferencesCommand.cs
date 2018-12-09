using System;
using System.IO;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.Serialization.CLI.Utils
{
    public abstract class AbstractPreferencesCommand : AbstractCommand
    {
        protected readonly Logger _logger;
        protected Preferences _preferences;

        protected AbstractPreferencesCommand(string loggerName)
        {
            _logger = fabl.GetLogger(loggerName);
        }

        public override void Run(CLIProgram program, string[] args)
        {
            try
            {
                var propertiesPath = args.GetPropertiesPath();
                if (!File.Exists(propertiesPath))
                {
                    throw new Exception("The file " + propertiesPath + " does not exist.");
                }

                var userPropertiesPath = args.GetUserPropertiesPath();
                if (userPropertiesPath != null && !File.Exists(userPropertiesPath))
                {
                    throw new Exception("The file " + userPropertiesPath + " does not exist.");
                }

                _preferences = new Preferences(propertiesPath, userPropertiesPath);
                (this as IConfigurable)?.Configure(_preferences);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                return;
            }

            base.Run(program, args);
        }
    }
}
