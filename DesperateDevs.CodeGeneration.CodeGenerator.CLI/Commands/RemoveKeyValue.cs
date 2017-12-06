using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class RemoveKeyValue : AbstractPreferencesCommand {

        public override string trigger { get { return "remove"; } }
        public override string description { get { return "Remove a key or a value from a key"; } }
        public override string example { get { return "jenny remove key value"; } }

        public RemoveKeyValue() : base(typeof(RemoveKeyValue).Name) {
        }

        protected override void run() {
            if (_args.Length == 2) {
                removeValue(_args[0], _args[1]);
            } else if (_args.Length == 1) {
                removeKey(_args[0]);
            } else {
                _logger.Warn("The remove command expects one or two arguments");
                _logger.Info("E.g. jenny remove myKey myValue");
            }
        }

        void removeValue(string key, string value) {
            if (_preferences.HasKey(key)) {
                APIUtil.RemoveValue(
                    value,
                    _preferences[key].ArrayFromCSV(),
                    values => _preferences[key] = values.ToCSV(),
                    _preferences);
            } else {
                _logger.Warn("Key doesn't exist: " + key);
            }
        }

        void removeKey(string key) {
            if (_preferences.HasKey(key)) {
                APIUtil.AskRemoveKey(
                    "Do you want to remove",
                    key,
                    _preferences);
            } else {
                _logger.Warn("Key doesn't exist: " + key);
            }
        }
    }
}
