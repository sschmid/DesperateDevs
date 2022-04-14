using System;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public class SetKeyValueCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "set";
        public override string Description => "Set the value of a key";
        public override string Group => CommandGroups.Properties;
        public override string Example => "set [key] [value]";

        public SetKeyValueCommand() : base(typeof(SetKeyValueCommand).FullName) { }

        protected override void Run()
        {
            if (_args.Length == 2)
                SetKeyValue(_args[0], _args[1]);
            else
                _logger.Error("The set command expects exactly two arguments, e.g. 'set myKey myValue'");
        }

        void SetKeyValue(string key, string value)
        {
            if (_preferences.HasKey(key))
                _preferences.AddValue(
                    value,
                    Array.Empty<string>(),
                    values => _preferences[key] = values.ToCSV(false, true)
                );
            else
                _preferences.AskAddKey("Key doesn't exist. Do you want to add", key, value);
        }
    }
}
