﻿using System.Linq;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.CLI.Utils
{
    public class DumpCommand : AbstractPreferencesCommand
    {
        public override string trigger => "dump";
        public override string description => "List all config keys and values";
        public override string group => CommandGroups.PROPERTIES;
        public override string example => "dump";

        public DumpCommand() : base(typeof(DumpCommand).FullName)
        {
        }

        protected override void run()
        {
            _logger.Debug(_preferences.ToString());

            const string indent = "\n├── ";
            const string lastIndent = "\n└── ";
            foreach (var key in _preferences.Keys)
            {
                var values = _preferences[key].FromCSV(true).ToArray();

                string valueString;
                if (values.Length > 1)
                {
                    valueString = indent + string.Join(indent, values.Take(values.Length - 1).ToArray()) +
                                  lastIndent + values.Last();
                }
                else if (values.Length == 1)
                {
                    valueString = lastIndent + values[0];
                }
                else
                {
                    valueString = string.Empty;
                }

                _logger.Info(key + valueString);
            }
        }
    }
}
