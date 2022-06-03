using System.Linq;
using System.Text;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class DumpCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "dump";
        public override string Description => "List all config keys and values";
        public override string Group => CommandGroups.Properties;
        public override string Example => "dump";

        public DumpCommand() : base(typeof(DumpCommand).FullName) { }

        protected override void Run()
        {
            _logger.Debug(_preferences.ToString());

            const string singleValue = " = ";
            const string indent = "\n├── ";
            const string lastIndent = "\n└── ";

            var sb = new StringBuilder();
            foreach (var key in _preferences.Keys)
            {
                sb.Append(key);
                var values = _preferences[key].FromCSV(true).ToArray();
                if (values.Length > 1)
                {
                    sb.AppendLine(indent + string.Join(indent, values.Take(values.Length - 1)) +
                                  lastIndent + values.Last());
                }
                else if (values.Length == 1)
                {
                    sb.Append(singleValue);
                    sb.AppendLine(values[0]);
                }
            }

            _logger.Info(sb.ToString());
        }
    }
}
