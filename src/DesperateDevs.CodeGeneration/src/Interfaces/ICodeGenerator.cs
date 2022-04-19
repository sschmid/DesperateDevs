namespace DesperateDevs.CodeGeneration
{
    public interface ICodeGenerator : ICodeGenerationPlugin
    {
        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
