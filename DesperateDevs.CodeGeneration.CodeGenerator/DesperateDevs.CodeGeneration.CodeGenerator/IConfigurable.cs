using System.Collections.Generic;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public interface IConfigurable {

        Dictionary<string, string> defaultProperties { get; }

        void Configure(Preferences preferences);
    }
}
