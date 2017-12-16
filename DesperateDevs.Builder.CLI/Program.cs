using System;
using DesperateDevs.CLI;

namespace DesperateDevs.Builder.CLI {

    class Program {

        static CLIProgram _program;

        public static void Main(string[] args) {
            _program = new CLIProgram("Builder");
            _program.Run(args, printUsage);
        }

        static void printUsage(ICommand[] commands) {
            var pad = _program.GetCommandListPad();
            var commandList = _program.GetFormattedCommandList();

            commandList.Add("[-v]".PadRight(pad) + " - verbose output");

            const string header = @"Builder C#";
            const string footer = "EXAMPLE\n" +
                                  "  builder version minor";

            Console.WriteLine(header);
            Console.WriteLine("usage:\n" + string.Join("\n", commandList));
            Console.WriteLine("\n" + footer);
        }
    }
}
