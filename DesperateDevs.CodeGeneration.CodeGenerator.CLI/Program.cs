using System;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            AbstractPreferencesCommand.defaultPropertiesPath = "Jenny.properties";
            AbstractPreferencesCommand.defaultUserPropertiesPath = Environment.UserName + ".userproperties";
            new CLIProgram("Jenny", typeof(WizardCommand), args).Run();
        }
    }
}
