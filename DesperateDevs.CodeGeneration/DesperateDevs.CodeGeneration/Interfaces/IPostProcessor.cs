namespace DesperateDevs.CodeGeneration {

    public interface IPostProcessor : ICodeGeneratorBase {

        CodeGenFile[] PostProcess(CodeGenFile[] files);
    }
}
