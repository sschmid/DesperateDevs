using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using Jenny;

namespace Samples.Jenny
{
    public class SampleDataProvider : IDataProvider, IConfigurable
    {
        public string Name => "Sample";
        public int Order => 0;
        public bool RunInDryMode => true;

        readonly string _countKey = $"{typeof(SampleDataProvider).Namespace}.Count";

        int _count;

        public Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {_countKey, "5"}
        };

        public void Configure(Preferences preferences)
        {
            _count = int.Parse(preferences[_countKey]);
        }

        public CodeGeneratorData[] GetData() => Enumerable
            .Range(1, _count)
            .Select(i => new CodeGeneratorData
            {
                ["File.Name"] = $"File{i}.cs",
                ["File.Content"] = $"public class File{i} {{}}"
            })
            .ToArray();
    }
}
