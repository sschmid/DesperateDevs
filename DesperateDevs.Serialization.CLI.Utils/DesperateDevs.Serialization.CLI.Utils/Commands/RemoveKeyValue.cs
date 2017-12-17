﻿using DesperateDevs.Utils;

namespace DesperateDevs.Serialization.CLI.Utils {

    public class RemoveKeyValue : AbstractPreferencesCommand {

        public override string trigger { get { return "remove"; } }
        public override string description { get { return "Remove a key or a value from a key"; } }
        public override string example { get { return "remove key value"; } }

        public RemoveKeyValue() : base(typeof(RemoveKeyValue).FullName) {
        }

        protected override void run() {
            if (_args.Length == 2) {
                removeValue(_args[0], _args[1]);
            } else if (_args.Length == 1) {
                removeKey(_args[0]);
            } else {
                _logger.Warn("The remove command expects one or two arguments");
                _logger.Info("E.g. remove myKey myValue");
            }
        }

        void removeValue(string key, string value) {
            if (_preferences.HasKey(key)) {
                _preferences.RemoveValue(
                    value,
                    _preferences[key].ArrayFromCSV(),
                    values => _preferences[key] = values.ToCSV());
            } else {
                _logger.Warn("Key doesn't exist: " + key);
            }
        }

        void removeKey(string key) {
            if (_preferences.HasKey(key)) {
                _preferences.AskRemoveKey(
                    "Do you want to remove",
                    key);
            } else {
                _logger.Warn("Key doesn't exist: " + key);
            }
        }
    }
}
