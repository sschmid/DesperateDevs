namespace DesperateDevs.CodeGen
{
    public interface IDoctor : ICodeGenerationPlugin
    {
        Diagnosis Diagnose();
        bool ApplyFix();
    }
}
