using System;
using System.IO;
using System.Linq;
using DesperateDevs.Logging;

namespace DesperateDevs.Serialization {

    public class Preferences {

        public static Preferences sharedInstance {
            get {
                if (_sharedInstance == null) {
                    _sharedInstance = new Preferences((string)null , null);
                }

                _sharedInstance.Reload();
                return _sharedInstance;
            }
            set { _sharedInstance = value; } }

        static Preferences _sharedInstance;

        public string propertiesPath { get { return _propertiesPath; } }
        public string userPropertiesPath { get { return _userPropertiesPath; } }

        public bool propertiesExist { get { return File.Exists(_propertiesPath); } }
        public bool userPropertiesExist { get { return File.Exists(_userPropertiesPath); } }

        public string[] keys { get { return getMergedProperties().keys; } }

        public Properties properties { get { return _properties; } }
        public Properties userProperties { get { return _userProperties; } }

        static readonly Logger _logger = fabl.GetLogger(typeof(Preferences).FullName);

        readonly string _propertiesPath;
        readonly string _userPropertiesPath;

        Properties _properties;
        Properties _userProperties;

        public Preferences(string propertiesPath, string userPropertiesPath) {
            _propertiesPath = propertiesPath
                              ?? findFilePath("*.properties")
                              ?? "Jenny.properties";

            _userPropertiesPath = userPropertiesPath
                                  ?? findFilePath("*.userproperties")
                                  ?? Environment.UserName + ".userproperties";

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

        public override string ToString() {
            return getMergedProperties().ToString();
        }

        public static string[] FindAll(string searchPattern) {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), searchPattern, SearchOption.TopDirectoryOnly);
        }

        static string findFilePath(string searchPattern) {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), searchPattern, SearchOption.TopDirectoryOnly);
            var file = files.FirstOrDefault();
            if (files.Length > 1) {
                _logger.Warn(
                    "Found multiple files matching " + searchPattern + ":\n" +
                    string.Join("\n", files.Select(Path.GetFileName).ToArray()) + "\n" +
                    "Using " + file
                );
            }

            return file;
        }

        static Properties loadProperties(string path) {
            return new Properties(File.Exists(path)
                ? File.ReadAllText(path)
                : string.Empty
            );
        }

        Properties getMergedProperties() {
            var mergedProperties = new Properties(_properties.ToDictionary());
            mergedProperties.AddProperties(_userProperties.ToDictionary(), true);
            return mergedProperties;
        }
    }
}
