using System;
using System.Collections.Generic;
using System.IO;

namespace DesperateDevs.Serialization
{
    public class Preferences
    {
        public static string DefaultUserPropertiesPath => Environment.UserName + ".userproperties";

        public readonly string PropertiesPath;
        public readonly string UserPropertiesPath;

        public IEnumerable<string> Keys => _mergedProperties.Keys;

        public bool Minified { get; set; }
        public Properties Properties { get; private set; }
        public Properties UserProperties { get; private set; }

        readonly bool _doubleQuotedValues;

        Properties _mergedProperties;

        public Preferences(string propertiesPath, string userPropertiesPath, bool doubleQuotedValues = false) : this(doubleQuotedValues)
        {
            PropertiesPath = propertiesPath;
            UserPropertiesPath = userPropertiesPath ?? DefaultUserPropertiesPath;
            Reload();
        }

        protected Preferences(Properties properties, Properties userProperties, bool doubleQuotedValues = false) : this(doubleQuotedValues)
        {
            Properties = properties;
            UserProperties = userProperties;
            CreateMergedProperties();
        }

        Preferences(bool doubleQuotedValues) => _doubleQuotedValues = doubleQuotedValues;

        public string this[string key]
        {
            get => _mergedProperties[key];
            set
            {
                Properties[key] = value;
                _mergedProperties[key] = value;
            }
        }

        public bool TryGetValue(string key, out string value) => _mergedProperties.TryGetValue(key, out value);

        public void Reload()
        {
            Properties = LoadProperties(PropertiesPath);
            UserProperties = LoadProperties(UserPropertiesPath);
            CreateMergedProperties();
        }

        void CreateMergedProperties()
        {
            _mergedProperties = new Properties(_doubleQuotedValues);
            _mergedProperties.AddProperties(Properties.ToDictionary(), true);
            _mergedProperties.AddProperties(UserProperties.ToDictionary(), true);
        }

        public void Save()
        {
            File.WriteAllText(PropertiesPath, Minified ? Properties.ToMinifiedString() : Properties.ToString());
            File.WriteAllText(UserPropertiesPath, Minified ? UserProperties.ToMinifiedString() : UserProperties.ToString());
        }

        public bool HasKey(string key) => _mergedProperties.HasKey(key);

        public void Clear(bool resetUser = false)
        {
            Properties = new Properties(_doubleQuotedValues);
            if (resetUser)
                UserProperties = new Properties(_doubleQuotedValues);
            CreateMergedProperties();
        }

        public string ToMinifiedString() => _mergedProperties.ToMinifiedString();
        public override string ToString() => _mergedProperties.ToString();

        Properties LoadProperties(string path) => new Properties(
            File.Exists(path)
                ? File.ReadAllText(path)
                : string.Empty,
            _doubleQuotedValues);
    }
}
