using System.Collections.Generic;

namespace DesperateDevs.Serialization {

    public abstract class AbstractConfigurableConfig : IConfigurable {

        public abstract Dictionary<string, string> defaultProperties { get; }

        protected Preferences _preferences;

        public virtual void Configure(Preferences preferences) {
            _preferences = preferences;
        }

        public override string ToString() {
            return _preferences.ToString();
        }
    }
}
