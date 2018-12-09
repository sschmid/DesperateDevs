using DesperateDevs.CLI.Utils;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            AbstractPreferencesCommand.defaultPropertiesPath = CodeGenerator.defaultPropertiesPath;
            new CLIProgram("Jenny", typeof(WizardCommand), args).Run();
        }
    }
}
