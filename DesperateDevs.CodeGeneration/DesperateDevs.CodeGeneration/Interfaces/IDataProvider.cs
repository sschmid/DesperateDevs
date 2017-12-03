namespace DesperateDevs.CodeGeneration {

    public interface IDataProvider : ICodeGeneratorBase {

        CodeGeneratorData[] GetData();
    }
}
