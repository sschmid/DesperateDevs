using System;
using DesperateDevs.Cli.Utils;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class RemoveKeyValueCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "remove";
        public override string Description => "Remove a key or a value from a key";
        public override string Group => CommandGroups.Properties;
        public override string Example => "remove [key] [value]";

        public RemoveKeyValueCommand() : base(typeof(RemoveKeyValueCommand).FullName) { }

        protected override void Run()
        {
            if (_args.Length == 2)
                RemoveValue(_args[0], _args[1]);
            else if (_args.Length == 1)
                RemoveKey(_args[0]);
            else
                throw new Exception("The remove command expects one or two arguments, e.g. 'remove myKey myValue'");
        }

        void RemoveValue(string key, string value)
        {
            if (_preferences.HasKey(key))
            {
                _preferences.RemoveValue(
                    value,
                    _preferences[key].FromCSV(true),
                    values => _preferences[key] = values.ToCSV(false, true)
                );
            }
            else
            {
                _logger.Warn($"Key doesn't exist: {key}");
            }
        }

        void RemoveKey(string key)
        {
            if (_preferences.HasKey(key))
            {
                if (_rawArgs.IsYes())
                    _preferences.RemoveKey(key);
                else
                    _preferences.AskRemoveKey("Do you want to remove", key);
            }
            else
            {
                _logger.Warn($"Key doesn't exist: {key}");
            }
        }
    }
}
