using System;
using DesperateDevs.CLI;

namespace DesperateDevs.Networking.CLI {

    class Program {

        static CLIProgram _program;

        public static void Main(string[] args) {
            _program = new CLIProgram("TCPezy");
            _program.Run(args, printUsage);
        }

        static void printUsage(ICommand[] commands) {
            var pad = _program.GetCommandListPad();
            var commandList = _program.GetFormattedCommandList();

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
