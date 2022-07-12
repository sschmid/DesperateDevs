using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Cli.Utils;
using DesperateDevs.Extensions;
using Sherlog;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class NewPreferencesCommand : AbstractCommand
    {
        public override string Trigger => "new";
        public override string Description => "Create new properties file(s) with default values";
        public override string Group => CommandGroups.Properties;
        public override string Example => "new [file] [userFile] [-f]";

        readonly Logger _logger = Logger.GetLogger(typeof(NewPreferencesCommand));

        protected override void Run()
        {
            var properties = _args.GetPropertiesPath();
            var userProperties = _args.GetUserPropertiesPath();

            if (!_rawArgs.IsForce() && (Exists(properties) || Exists(userProperties)))
                return;

            var preferences = new Preferences(properties, userProperties);
            preferences.Clear(true);
            var defaultProperties = new Dictionary<string, string>().Merge(
                AppDomain.CurrentDomain
                    .GetInstancesOf<IConfigurable>()
                    .Select(instance => instance.DefaultProperties));

            preferences.Properties.AddProperties(defaultProperties, true);
            preferences.Save();

            _logger.Info($"Created {preferences.PropertiesPath}");
            _logger.Info($"Created {preferences.UserPropertiesPath}");
            _logger.Debug(preferences.ToString());

            if (!_rawArgs.IsSilent())
                new EditConfigCommand().Run(_program, _args);
        }

        bool Exists(string path)
        {
            if (File.Exists(path))
            {
                _logger.Warn($"{path} already exists!");
                _logger.Info("Use 'new -f' to overwrite the exiting file.");
                _logger.Info("Use 'edit' to open the exiting file.");

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
