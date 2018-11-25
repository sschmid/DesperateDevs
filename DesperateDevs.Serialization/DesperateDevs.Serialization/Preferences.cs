using System.IO;

namespace DesperateDevs.Serialization {

    public class Preferences {

        public string propertiesPath { get { return _propertiesPath; } }
        public string userPropertiesPath { get { return _userPropertiesPath; } }

        public string[] keys { get { return getMergedProperties().keys; } }

        public Properties properties { get { return _properties; } }
        public Properties userProperties { get { return _userProperties; } }

        readonly string _propertiesPath;
        readonly string _userPropertiesPath;

        Properties _properties;
        Properties _userProperties;

        bool _isDoubleQuoteMode;

        public Preferences(string propertiesPath, string userPropertiesPath) {
            _propertiesPath = propertiesPath;
            _userPropertiesPath = userPropertiesPath;
            Reload();
        }

        protected Preferences(Properties properties, Properties userProperties) {
            _properties = properties;
            _userProperties = userProperties;
        }

        public void Reload() {
            _properties = loadProperties(_propertiesPath);
            _userProperties = loadProperties(_userPropertiesPath);
        }

        public void Save(bool minified = false) {
            File.WriteAllText(_propertiesPath, minified ? _properties.ToMinifiedString() : _properties.ToString());
            File.WriteAllText(_userPropertiesPath, minified ? _userProperties.ToMinifiedString() : _userProperties.ToString());
        }

        public string this[string key] {
            get { return getMergedProperties()[key]; }
            set {
                if (!_properties.HasKey(key) || value != this[key]) {
                    _properties[key] = value;
                }
            }
        }

        public bool HasKey(string key) {
            return _properties.HasKey(key) || _userProperties.HasKey(key);
        }

        public void Reset(bool resetUser = false) {
            _properties = new Properties();
            if (resetUser) {
                _userProperties = new Properties();
            }
        }

        public void EnableDoubleQuoteMode() {
            _isDoubleQuoteMode = true;
            _properties.EnableDoubleQuoteMode();
            _userProperties.EnableDoubleQuoteMode();
        }

        public override string ToString() {
            return getMergedProperties().ToString();
        }

        static Properties loadProperties(string path) {
            return new Properties(File.Exists(path)
                ? File.ReadAllText(path)
                : string.Empty
            );
        }

        Properties getMergedProperties() {
            var mergedProperties = new Properties(_properties.ToDictionary());
            if (_isDoubleQuoteMode) {
                mergedProperties.EnableDoubleQuoteMode();
            }

            mergedProperties.AddProperties(_userProperties.ToDictionary(), true);
            return mergedProperties;
        }
    }
}
