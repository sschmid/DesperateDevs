namespace DesperateDevs.CodeGen
{
    public interface IPostProcessor : ICodeGenerationPlugin
    {
        CodeGenFile[] PostProcess(CodeGenFile[] files);
    }
}
