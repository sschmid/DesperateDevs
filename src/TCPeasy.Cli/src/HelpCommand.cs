using System;
using DesperateDevs.Cli.Utils;

namespace TCPeasy.Cli
{
    public class HelpCommand : AbstractCommand
    {
        public override string Trigger => "help";
        public override string Description => "Show help";
        public override string Group => null;
        public override string Example => "help";

        protected override void Run()
        {
            var commandList = _program.GetFormattedCommandList();
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
