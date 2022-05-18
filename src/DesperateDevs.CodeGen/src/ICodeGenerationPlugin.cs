namespace DesperateDevs.CodeGen
{
    public interface ICodeGenerationPlugin
    {
        string Name { get; }
        int Order { get; }
        bool RunInDryMode { get; }
    }
}
