using System;
using System.IO;
using DesperateDevs.Cli.Utils;
using Sherlog;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public abstract class AbstractPreferencesCommand : AbstractCommand
    {
        public static string DefaultPropertiesPath;

        protected readonly Logger _logger;
        protected Preferences _preferences;

        protected AbstractPreferencesCommand(string loggerName)
        {
            _logger = Logger.GetLogger(loggerName);
        }

        public override void Run(CliProgram program, string[] args)
        {
            try
            {
                var propertiesPath = args.GetPropertiesPath() ?? DefaultPropertiesPath;
                if (!File.Exists(propertiesPath))
                    throw new Exception($"The file {propertiesPath} does not exist.");

                var userPropertiesPath = args.GetUserPropertiesPath();
                if (userPropertiesPath != null && !File.Exists(userPropertiesPath))
                    throw new Exception($"The file {userPropertiesPath} does not exist.");

                _preferences = new Preferences(propertiesPath, userPropertiesPath);

                if (this is IConfigurable configurable)
                    configurable.Configure(_preferences);
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
