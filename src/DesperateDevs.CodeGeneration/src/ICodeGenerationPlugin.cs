namespace DesperateDevs.CodeGeneration
{
    public interface ICodeGenerationPlugin
    {
        string Name { get; }
        int Priority { get; }
        bool RunInDryMode { get; }
    }
}
