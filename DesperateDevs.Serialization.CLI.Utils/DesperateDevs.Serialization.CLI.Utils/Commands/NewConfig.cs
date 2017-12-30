using System.IO;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.Serialization.CLI.Utils {

    public class NewConfig : AbstractCommand {

        public override string trigger { get { return "new"; } }
        public override string description { get { return "Create new properties file(s) with default values"; } }
        public override string example { get { return "new file userFile [-f]"; } }

        readonly Logger _logger = fabl.GetLogger(typeof(NewConfig));

        protected override void run() {
            var currentDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
            var properties = currentDir + _args.GetPropertiesPath();
            var userProperties = currentDir + _args.GetUserPropertiesPath();

            if (!_args.isForce() && (doesAlreadyExist(properties) || doesAlreadyExist(userProperties))) {
                return;
            }

            var preferences = new Preferences(properties, userProperties);
            preferences.Reset(true);
            var defaultProperties = CLIUtil.GetDefaultProperties();
            preferences.properties.AddProperties(defaultProperties, true);
            preferences.Save();

            _logger.Info("Created " + preferences.propertiesPath);
            _logger.Info("Created " + preferences.userPropertiesPath);
            _logger.Debug(preferences.ToString());

            new EditConfig().Run(_args);
        }

        bool doesAlreadyExist(string path) {
            if (File.Exists(path)) {
                _logger.Warn(path + " already exists!");
                _logger.Info("Use new -f to overwrite the exiting file.");
                _logger.Info("Use edit to open the exiting file.");

                return true;
            }

            return false;
        }
    }
}
