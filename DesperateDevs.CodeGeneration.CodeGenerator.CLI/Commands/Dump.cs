using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Dump : AbstractPreferencesCommand {

        public override string trigger { get { return "dump"; } }
        public override string description { get { return "List all config keys and values"; } }
        public override string example { get { return "jenny dump"; } }

        public Dump() : base(typeof(Dump).Name) {
        }

        protected override void run() {
            _logger.Debug(_preferences.ToString());

            const string indent = "\n    ";
            foreach (var key in _preferences.keys) {
                _logger.Info(key + indent + string.Join(indent, _preferences[key].ArrayFromCSV()));
            }
        }
    }
}

