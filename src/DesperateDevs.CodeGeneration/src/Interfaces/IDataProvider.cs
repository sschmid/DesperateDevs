namespace DesperateDevs.CodeGeneration
{
    public interface IDataProvider : ICodeGenerationPlugin
    {
        CodeGeneratorData[] GetData();
    }
}
