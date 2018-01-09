using System;
using DesperateDevs.CLI.Utils;

namespace DesperateDevs.Networking.CLI {

    class Program {

        public static void Main(string[] args) {
            new CLIProgram("TCPezy", args, printUsage);
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

   Connecting sockets since 2013
";

            const string footer = "EXAMPLE\n" +
                                  "  pezy listen 1234\n" +
                                  "  pezy connect localhost 1234";

            Console.WriteLine(header);
            Console.WriteLine("usage:\n" + string.Join("\n", commandList));
            Console.WriteLine("\n" + footer);
        }
    }
}
