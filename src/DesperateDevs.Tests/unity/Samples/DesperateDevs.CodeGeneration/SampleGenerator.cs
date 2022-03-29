using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Samples.DesperateDevs.CodeGeneration
{
    public class SampleGenerator : ICodeGenerator
    {
        public string name => "Sample";
        public int priority => 0;
        public bool runInDryMode => true;

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .Select(CreateFile)
            .ToArray();

        CodeGenFile CreateFile(CodeGeneratorData data) => new CodeGenFile(
            data["File.Name"].ToString(),
            data["File.Content"].ToString(),
            GetType().FullName
        );
    }
}
