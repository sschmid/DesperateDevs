using System.Linq;
using DesperateDevs.CodeGeneration;

public class ExampleGenerator : ICodeGenerator {

    public string name { get { return "Example"; } }
    public int priority { get { return 0; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select(createFile)
            .ToArray();
    }

    CodeGenFile createFile(CodeGeneratorData data) {
        return new CodeGenFile(
            data["File.Name"].ToString(),
            data["File.Content"].ToString(),
            GetType().FullName
        );
    }
}
