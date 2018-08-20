using DesperateDevs.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    class Program {

        public static void Main(string[] args) {
            new CLIProgram("Jenny", typeof(WizardCommand), args).Run();
        }
    }
}
