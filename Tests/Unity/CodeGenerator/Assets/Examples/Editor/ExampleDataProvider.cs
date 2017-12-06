using DesperateDevs.CodeGeneration;

public class ExampleDataProvider : IDataProvider {

    public string name { get { return "Example"; } }
    public int priority { get { return 0; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGeneratorData[] GetData() {
        var d1 = new CodeGeneratorData();
        d1["File.Name"] = "File1.cs";
        d1["File.Content"] = "public class File1 { }";

        var d2 = new CodeGeneratorData();
        d2["File.Name"] = "File2.cs";
        d2["File.Content"] = "public class File2 { }";

        return new[] { d1, d2 };
    }
}
