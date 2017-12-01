using System;
using DesperateDevs.CLI;

namespace DesperateDevs.Networking.CLI {

    class Program {

        public static void Main(string[] args) {
            new CLIProgram().Run(args, typeof(Program).Assembly, printUsage);
        }

        static void printUsage(ICommand[] commands) {
            var pad = CLIProgram.GetCommandListPad(commands);
            var commandList = CLIProgram.GetFormattedCommandList(commands);

            commandList.Add("[-v]".PadRight(pad) + " - verbose output");


            const string header = @"
████████╗ ██████╗██████╗ ███████╗███████╗██╗   ██╗
╚══██╔══╝██╔════╝██╔══██╗██╔════╝╚══███╔╝╚██╗ ██╔╝
   ██║   ██║     ██████╔╝█████╗    ███╔╝  ╚████╔╝
   ██║   ██║     ██╔═══╝ ██╔══╝   ███╔╝    ╚██╔╝
   ██║   ╚██████╗██║     ███████╗███████╗   ██║
   ╚═╝    ╚═════╝╚═╝     ╚══════╝╚══════╝   ╚═╝
";

            Console.WriteLine(header);
            Console.WriteLine("usage:\n{0}", string.Join("\n", commandList));
        }
    }
}
