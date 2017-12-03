using System.Collections.Generic;

namespace DesperateDevs.Serialization {

    public interface IConfigurable {

        Dictionary<string, string> defaultProperties { get; }

        void Configure(Preferences preferences);
    }
}
