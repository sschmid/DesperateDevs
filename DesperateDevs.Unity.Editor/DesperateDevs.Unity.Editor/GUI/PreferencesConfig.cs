using System.Collections.Generic;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.Unity.Editor {

    public class PreferencesConfig : AbstractConfigurableConfig {

        const string PREFERENCES_KEY = "Preferences.";

        readonly string _key;

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { _key , string.Empty }
                };
            }
        }

        public PreferencesConfig(string name) {
            _key = PREFERENCES_KEY + name;
        }

        public string[] preferenceDrawers {
            get { return _preferences[_key ].ArrayFromCSV(); }
            set { _preferences[_key ] = value.ToCSV(); }
        }
    }
}
