using System;
using DesperateDevs.CLI;

namespace DesperateDevs.Networking.CLI {

    class Program {

        static CLIProgram _program;

        public static void Main(string[] args) {
            _program = new CLIProgram("TCPezy", typeof(Program).Assembly);
            _program.Run(args, printUsage);
        }

        static void printUsage(ICommand[] commands) {
            var pad = _program .GetCommandListPad();
            var commandList = _program .GetFormattedCommandList();

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
