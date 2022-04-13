using System;
using System.IO;
using DesperateDevs.Cli.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public abstract class AbstractPreferencesCommand : AbstractCommand
    {
        public static string defaultPropertiesPath;

        protected readonly Logger _logger;
        protected Preferences _preferences;

        protected AbstractPreferencesCommand(string loggerName)
        {
            _logger = Sherlog.GetLogger(loggerName);
        }

        public override void Run(CliProgram program, string[] args)
        {
            try
            {
                var propertiesPath = args.GetPropertiesPath() ?? defaultPropertiesPath;
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
            catch (Exception exception)
            {
                _logger.Error(exception.Message);

                return;
            }

            base.Run(program, args);
        }
    }
}
