using System;
using DesperateDevs.CLI;

namespace DesperateDevs.Logging.CLI {

    class Program {

        public static void Main(string[] args) {
            var program = new CLIProgram();
            program.Run(args, typeof(Program).Assembly, printUsage);
        }

        static void printUsage(ICommand[] commands) {
            var pad = CLIProgram.GetCommandListPad(commands);
            var commandList = CLIProgram.GetFormattedCommandList(commands);

            commandList.Add("[-v]".PadRight(pad) + " - verbose output");

            Console.WriteLine("usage:\n{0}", string.Join("\n", commandList));
        }
    }
}
