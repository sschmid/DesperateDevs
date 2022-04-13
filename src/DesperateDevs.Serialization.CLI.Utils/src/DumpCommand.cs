using System.Linq;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class DumpCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "dump";
        public override string Description => "List all config keys and values";
        public override string Group => CommandGroups.PROPERTIES;
        public override string Example => "dump";

        public DumpCommand() : base(typeof(DumpCommand).FullName) { }

        protected override void Run()
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
