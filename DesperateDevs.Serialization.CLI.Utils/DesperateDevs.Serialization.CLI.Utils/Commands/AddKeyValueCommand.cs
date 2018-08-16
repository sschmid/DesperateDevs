﻿using DesperateDevs.Utils;

namespace DesperateDevs.Serialization.CLI.Utils {

    public class AddKeyValueCommand : AbstractPreferencesCommand {

        public override string trigger { get { return "add"; } }
        public override string description { get { return "Add a value to a key"; } }
        public override string group { get { return "Properties"; } }
        public override string example { get { return "add [key] [value]"; } }

        public AddKeyValueCommand() : base(typeof(AddKeyValueCommand).FullName) {
        }

        protected override void run() {
            if (_args.Length == 2) {
                addKeyValue(_args[0], _args[1]);
            } else {
                _logger.Error("The add command expects exactly two arguments");
                _logger.Info("E.g. add myKey myValue");
            }
        }

        void addKeyValue(string key, string value) {
            if (_preferences.HasKey(key)) {
                _preferences.AddValue(
                    value,
                    _preferences[key].ArrayFromCSV(),
                    values => _preferences[key] = values.ToCSV());
            } else {
                _preferences.AskAddKey("Key doesn't exist. Do you want to add", key, value);
            }
        }
    }
}
