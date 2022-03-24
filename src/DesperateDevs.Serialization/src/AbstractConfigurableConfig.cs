using System.Collections.Generic;

namespace DesperateDevs.Serialization
{
    public static class ConfigurableConfigExtension
    {
        public static T CreateAndConfigure<T>(this Preferences preferences) where T : IConfigurable, new()
        {
            var config = new T();
            config.Configure(preferences);
            return config;
        }
    }

    public abstract class AbstractConfigurableConfig : IConfigurable
    {
        public abstract Dictionary<string, string> DefaultProperties { get; }

        protected Preferences _preferences;

        public virtual void Configure(Preferences preferences) => _preferences = preferences;

        public override string ToString() => _preferences.ToString();
    }
}
