using DesperateDevs.Cli.Utils;
using DesperateDevs.Serialization.Cli.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            AbstractPreferencesCommand.defaultPropertiesPath = CodeGenerator.defaultPropertiesPath;
            new CliProgram("Jenny", typeof(WizardCommand), args).Run();
        }
    }
}
