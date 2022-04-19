namespace DesperateDevs.CodeGeneration
{
    public interface IDoctor : ICodeGenerationPlugin
    {
        Diagnosis Diagnose();
        bool Fix();
    }
}
