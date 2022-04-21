using DesperateDevs.CodeGeneration;

namespace Samples.DesperateDevs.CodeGeneration
{
    public class SampleDataProvider : IDataProvider
    {
        public string Name => "Sample";
        public int Order => 0;
        public bool RunInDryMode => true;

        public CodeGeneratorData[] GetData() => new[]
        {
            new CodeGeneratorData
            {
                ["File.Name"] = "File1.cs",
                ["File.Content"] = "public class File1 { }"
            },
            new CodeGeneratorData
            {
                ["File.Name"] = "File2.cs",
                ["File.Content"] = "public class File2 { }"
            }
        };
    }
}
