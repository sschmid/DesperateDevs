using DesperateDevs.Cli.Utils;
using DesperateDevs.Serialization.Cli.Utils;

namespace DesperateDevs.CodeGen.CodeGenerator.Cli
{
    static class Program
    {
        public static void Main(string[] args)
        {
            AbstractPreferencesCommand.DefaultPropertiesPath = CodeGenerator.DefaultPropertiesPath;
            new CliProgram("Jenny", typeof(WizardCommand), args).Run();
        }
    }
}
