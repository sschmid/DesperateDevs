using DesperateDevs.Cli.Utils;

namespace DesperateDevs.Net.Cli
{
    class Program
    {
        public static void Main(string[] args)
        {
            new CLIProgram("TCPezy", typeof(HelpCommand), args).Run();
        }
    }
}
