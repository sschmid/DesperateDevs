namespace DesperateDevs.CodeGen
{
    public interface ICodeGenerator : ICodeGenerationPlugin
    {
        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
