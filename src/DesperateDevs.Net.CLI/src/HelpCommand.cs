using System;
using DesperateDevs.Cli.Utils;

namespace DesperateDevs.Net.Cli
{
    public class HelpCommand : AbstractCommand
    {
        public override string Trigger => "help";
        public override string Description => "Show help";
        public override string Group => null;
        public override string Example => "help";

        protected override void Run()
        {
            var pad = _program.GetCommandListPad();
            var commandList = _program.GetFormattedCommandList();
            var args = "[-v]".PadRight(pad) + "     verbose output\n" +
                       "[-s]".PadRight(pad) + "     silent (minimal output)";


            Console.WriteLine($@"
████████╗ ██████╗██████╗ ███████╗ █████╗ ███████╗██╗   ██╗
╚══██╔══╝██╔════╝██╔══██╗██╔════╝██╔══██╗██╔════╝╚██╗ ██╔╝
   ██║   ██║     ██████╔╝█████╗  ███████║███████╗ ╚████╔╝
   ██║   ██║     ██╔═══╝ ██╔══╝  ██╔══██║╚════██║  ╚██╔╝
   ██║   ╚██████╗██║     ███████╗██║  ██║███████║   ██║
   ╚═╝    ╚═════╝╚═╝     ╚══════╝╚═╝  ╚═╝╚══════╝   ╚═╝

🔌 Connecting sockets

usage: tcpeasy [-v | -s] <command> [<args>]
{commandList}
EXAMPLE
  tcpeasy listen 1234
  tcpeasy connect localhost 1234
");
        }
    }
}
