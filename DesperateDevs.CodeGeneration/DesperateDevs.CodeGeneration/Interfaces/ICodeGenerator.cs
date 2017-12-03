namespace DesperateDevs.CodeGeneration {

    public interface ICodeGenerator : ICodeGeneratorBase {

        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}
