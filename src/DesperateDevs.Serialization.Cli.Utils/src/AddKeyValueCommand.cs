using System.Linq;
using DesperateDevs.Cli.Utils;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class AddKeyValueCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "add";
        public override string Description => "Add a value to a key";
        public override string Group => CommandGroups.Properties;
        public override string Example => "add [key] [value]";

        public AddKeyValueCommand() : base(typeof(AddKeyValueCommand).FullName) { }

        protected override void Run()
        {
            if (_args.Length == 2)
                AddKeyValue(_args[0], _args[1]);
            else
                _logger.Error("The add command expects exactly two arguments, e.g. 'add myKey myValue'");
        }

        void AddKeyValue(string key, string value)
        {
            _preferences.TryGetValue(key, out var existingValues);
            if (existingValues != null || _rawArgs.IsYes())
            {
                _preferences.AddValue(
                    value,
                    existingValues?.FromCSV(true) ?? Enumerable.Empty<string>(),
                    values => _preferences[key] = values.ToCSV(false, true)
                );
            }
            else if (!_rawArgs.IsNo())
            {
                _preferences.AskAddKey("Key doesn't exist. Do you want to add", key, value);
            }
        }
    }
}
