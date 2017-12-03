using System;
using DesperateDevs.CLI;

namespace DesperateDevs.Networking.CLI {

    class Program {

        public static void Main(string[] args) {
            new CLIProgram("TCPezy").Run(args, typeof(Program).Assembly, printUsage);
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
            const string footer = "EXAMPLE\n" +
                                  "  pezy listen 1234\n" +
                                  "  pezy connect 127.0.0.1 1234";

            Console.WriteLine("usage:\n{0}", string.Join("\n", commandList) + "\n\n" + footer);
        }
    }
}
