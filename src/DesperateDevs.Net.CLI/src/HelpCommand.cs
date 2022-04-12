using System;
using DesperateDevs.CLI.Utils;

namespace DesperateDevs.Net.Cli
{
    public class HelpCommand : AbstractCommand
    {
        public override string trigger => "help";
        public override string description => "Show help";
        public override string group => null;
        public override string example => "help";

        protected override void run()
        {
            const string header = @"
████████╗ ██████╗██████╗ ███████╗███████╗██╗   ██╗
╚══██╔══╝██╔════╝██╔══██╗██╔════╝╚══███╔╝╚██╗ ██╔╝
   ██║   ██║     ██████╔╝█████╗    ███╔╝  ╚████╔╝
   ██║   ██║     ██╔═══╝ ██╔══╝   ███╔╝    ╚██╔╝
   ██║   ╚██████╗██║     ███████╗███████╗   ██║
   ╚═╝    ╚═════╝╚═╝     ╚══════╝╚══════╝   ╚═╝

   Connecting sockets since 2013
";

            var pad = _program.GetCommandListPad();
            var commandList = _program.GetFormattedCommandList();
            var args = "[-v]".PadRight(pad) + "   - verbose output\n" +
                       "[-s]".PadRight(pad) + "   - silent (minimal output)";

            const string examples = "EXAMPLE\n" +
                                    "  tcpezy listen 1234\n" +
                                    "  tcpezy connect localhost 1234";

            Console.WriteLine(header + "\nusage:\n" + commandList + "\n" + args + "\n\n" + examples);
        }
    }
}
