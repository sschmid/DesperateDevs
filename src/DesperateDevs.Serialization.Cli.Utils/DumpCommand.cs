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

            const string delimiter = ":";
            const string space = " ";
            const string indent = "\n- ";

            var sb = new StringBuilder();
            foreach (var key in _preferences.Keys)
            {
                sb.Append(key);
                sb.Append(delimiter);
                var values = _preferences[key].FromCSV(true).ToArray();
                if (values.Length > 1)
                {
                    sb.AppendLine(indent + string.Join(indent, values));
                }
                else if (values.Length == 1)
                {
                    sb.Append(space);
                    sb.AppendLine(values[0]);
                }
                else
                {
                    sb.AppendLine();
                }
            }

            _logger.Info(sb.ToString());
        }
    }
}
