using System.Linq;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class AddKeyValueCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "add";
        public override string Description => "Add a value to a key";
        public override string Group => CommandGroups.PROPERTIES;
        public override string Example => "add [key] [value]";

        public AddKeyValueCommand() : base(typeof(AddKeyValueCommand).FullName) { }

        protected override void Run()
        {
            if (_args.Length == 2)
            {
                addKeyValue(_args[0], _args[1]);
            }
            else
            {
                _logger.Error("The add command expects exactly two arguments");
                _logger.Info("E.g. add myKey myValue");
            }
        }

        void addKeyValue(string key, string value)
        {
            if (_preferences.HasKey(key))
            {
                _preferences.AddValue(
                    value,
                    _preferences[key].FromCSV(true).ToArray(),
                    values => _preferences[key] = values.ToCSV(false, true));
            }
            else
            {
                _preferences.AskAddKey("Key doesn't exist. Do you want to add", key, value);
            }
        }
    }
}
