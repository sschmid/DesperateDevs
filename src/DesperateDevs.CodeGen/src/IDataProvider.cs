namespace DesperateDevs.CodeGen
{
    public interface IDataProvider : ICodeGenerationPlugin
    {
        CodeGeneratorData[] GetData();
    }
}
