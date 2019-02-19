using System;
using System.IO;

namespace DesperateDevs.Serialization {

    public class Preferences {

        public static string defaultUserPropertiesPath { get { return Environment.UserName + ".userproperties"; } }

        public string propertiesPath { get { return _propertiesPath; } }
        public string userPropertiesPath { get { return _userPropertiesPath; } }

        public string[] keys { get { return getMergedProperties().keys; } }

        public Properties properties { get { return _properties; } }
        public Properties userProperties { get { return _userProperties; } }

        public bool doubleQuoteMode {
            get { return _isDoubleQuoteMode; }
            set {
                _isDoubleQuoteMode = value;
                _properties.doubleQuoteMode = value;
                _userProperties.doubleQuoteMode = value;
            }
        }

        public bool minified {
            get { return _isMinified; }
            set { _isMinified = value; }
        }

        readonly string _propertiesPath;
        readonly string _userPropertiesPath;

        Properties _properties;
        Properties _userProperties;

        bool _isDoubleQuoteMode;
        bool _isMinified;

        public Preferences(string propertiesPath, string userPropertiesPath) {
            _propertiesPath = propertiesPath;
            _userPropertiesPath = userPropertiesPath ?? defaultUserPropertiesPath;
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

        public void Save() {
            File.WriteAllText(_propertiesPath, _isMinified ? _properties.ToMinifiedString() : _properties.ToString());
            File.WriteAllText(_userPropertiesPath, _isMinified ? _userProperties.ToMinifiedString() : _userProperties.ToString());
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
            mergedProperties.doubleQuoteMode = _isDoubleQuoteMode;
            mergedProperties.AddProperties(_userProperties.ToDictionary(), true);
            return mergedProperties;
        }
    }
}
