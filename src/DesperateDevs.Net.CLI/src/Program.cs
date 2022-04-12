using DesperateDevs.Cli.Utils;

namespace DesperateDevs.Net.Cli
{
    class Program
    {
        public static void Main(string[] args)
        {
            new CliProgram("TCPezy", typeof(HelpCommand), args).Run();
        }
    }
}
