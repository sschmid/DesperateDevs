﻿using System.Linq;
using DesperateDevs.Utils;

namespace DesperateDevs.Serialization.CLI.Utils {

    public class Dump : AbstractPreferencesCommand {

        public override string trigger { get { return "dump"; } }
        public override string description { get { return "List all config keys and values"; } }
        public override string example { get { return "dump"; } }

        public Dump() : base(typeof(Dump).FullName) {
        }

        protected override void run() {
            _logger.Debug(_preferences.ToString());

            const string indent = "\n├── ";
            const string lastIndent = "\n└── ";
            foreach (var key in _preferences.keys) {
                var values = _preferences[key].ArrayFromCSV();

                string valueString;
                if (values.Length > 1) {
                    valueString = indent + string.Join(indent, values.Take(values.Length - 1).ToArray()) +
                                  lastIndent + values.Last();
                } else if (values.Length == 1) {
                    valueString = lastIndent + values[0];
                } else {
                    valueString = string.Empty;
                }

                _logger.Info(key + valueString);
            }
        }
    }
}
