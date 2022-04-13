using System.Linq;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class RemoveKeyValueCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "remove";
        public override string Description => "Remove a key or a value from a key";
        public override string Group => CommandGroups.PROPERTIES;
        public override string Example => "remove [key] [value]";

        public RemoveKeyValueCommand() : base(typeof(RemoveKeyValueCommand).FullName) { }

        protected override void Run()
        {
            if (_args.Length == 2)
            {
                removeValue(_args[0], _args[1]);
            }
            else if (_args.Length == 1)
            {
                removeKey(_args[0]);
            }
            else
            {
                _logger.Error("The remove command expects one or two arguments");
                _logger.Info("E.g. remove myKey myValue");
            }
        }

        void removeValue(string key, string value)
        {
            if (_preferences.HasKey(key))
            {
                _preferences.RemoveValue(
                    value,
                    _preferences[key].FromCSV(true).ToArray(),
                    values => _preferences[key] = values.ToCSV(false, true));
            }
            else
            {
                _logger.Warn("Key doesn't exist: " + key);
            }
        }

        void removeKey(string key)
        {
            if (_preferences.HasKey(key))
            {
                _preferences.AskRemoveKey(
                    "Do you want to remove",
                    key);
            }
            else
            {
                _logger.Warn("Key doesn't exist: " + key);
            }
        }
    }
}
