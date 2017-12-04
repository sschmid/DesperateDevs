using System;
using System.IO;
using DesperateDevs.CLI;
using DesperateDevs.Logging;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class NewConfig : AbstractCommand {

        public override string trigger { get { return "new"; } }
        public override string description { get { return "Create new properties file(s) with default values"; } }
        public override string example { get { return "jenny new file userFile [-f]"; } }

        static readonly Logger _logger = fabl.GetLogger(typeof(NewConfig).Name);

        protected override void run() {
            var currentDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
            var properties = currentDir + _args.GetPropertiesPath();
            var userProperties = currentDir + _args.GetUserPropertiesPath();

            if (!_args.isForce() && File.Exists(properties)) {
                _logger.Warn(properties + " already exists!");
                _logger.Info("Use jenny new -f to overwrite the exiting file.");
                _logger.Info("Use jenny edit to open the exiting file.");

                return;
            }

            if (!_args.isForce() && File.Exists(userProperties)) {
                _logger.Warn(userProperties + " already exists!");
                _logger.Info("Use jenny new -f to overwrite the exiting file.");
                _logger.Info("Use jenny edit to open the exiting file.");

                return;
            }

            var preferences = new Preferences(properties, userProperties);
            preferences.Reset(true);
            preferences.properties.AddProperties(new CodeGeneratorConfig().defaultProperties, true);
            preferences.properties.AddProperties(new CLIConfig().defaultProperties, true);
            preferences.Save();

            _logger.Info("Created " + preferences.propertiesPath);
            _logger.Info("Created " + preferences.userPropertiesPath);
            _logger.Info("👍");
            _logger.Debug(preferences.ToString());

            new EditConfig().Run(_args);
        }
    }
}
