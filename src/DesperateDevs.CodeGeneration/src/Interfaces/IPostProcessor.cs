namespace DesperateDevs.CodeGeneration
{
    public interface IPostProcessor : ICodeGenerationPlugin
    {
        CodeGenFile[] PostProcess(CodeGenFile[] files);
    }
}
