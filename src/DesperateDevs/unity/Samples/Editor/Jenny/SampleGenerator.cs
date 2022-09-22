using System.Linq;
using Jenny;

namespace Samples.Jenny
{
    public class SampleGenerator : ICodeGenerator
    {
        public string Name => "Sample";
        public int Order => 0;
        public bool RunInDryMode => true;

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
