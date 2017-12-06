using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class SetKeyValue : AbstractPreferencesCommand {

        public override string trigger { get { return "set"; } }
        public override string description { get { return "Set the value of a key"; } }
        public override string example { get { return "jenny set key value"; } }

        public SetKeyValue() : base(typeof(SetKeyValue).Name) {
        }

        protected override void run() {
            if (_args.Length == 2) {
                setKeyValue(_args[0], _args[1]);
            } else {
                _logger.Warn("The set command expects exactly two arguments");
                _logger.Info("E.g. jenny set myKey myValue");
            }
        }

        void setKeyValue(string key, string value) {
            if (_preferences.HasKey(key)) {
                APIUtil.AddValue(
                    value,
                    new string[0],
                    values => _preferences[key] = values.ToCSV(),
                    _preferences);
            } else {
                APIUtil.AskAddKey("Key doesn't exist. Do you want to add", key, value, _preferences);
            }
        }
    }
}
