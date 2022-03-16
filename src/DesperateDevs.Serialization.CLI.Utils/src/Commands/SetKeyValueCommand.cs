using DesperateDevs;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.CLI.Utils
{
    public class SetKeyValueCommand : AbstractPreferencesCommand
    {
        public override string trigger => "set";
        public override string description => "Set the value of a key";
        public override string group => CommandGroups.PROPERTIES;
        public override string example => "set [key] [value]";

        public SetKeyValueCommand() : base(typeof(SetKeyValueCommand).FullName)
        {
        }

        protected override void run()
        {
            if (_args.Length == 2)
            {
                setKeyValue(_args[0], _args[1]);
            }
            else
            {
                _logger.Error("The set command expects exactly two arguments");
                _logger.Info("E.g. set myKey myValue");
            }
        }

        void setKeyValue(string key, string value)
        {
            if (_preferences.HasKey(key))
            {
                _preferences.AddValue(
                    value,
                    new string[0],
                    values => _preferences[key] = values.ToCSV());
            }
            else
            {
                _preferences.AskAddKey("Key doesn't exist. Do you want to add", key, value);
            }
        }
    }
}
