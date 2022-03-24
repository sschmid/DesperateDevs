using System.Collections.Generic;

namespace DesperateDevs.Serialization
{
    public interface IConfigurable
    {
        Dictionary<string, string> DefaultProperties { get; }

        void Configure(Preferences preferences);
    }
}
