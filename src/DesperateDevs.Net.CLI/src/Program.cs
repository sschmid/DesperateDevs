using DesperateDevs.Cli.Utils;

namespace DesperateDevs.Net.Cli
{
    static class Program
    {
        public static void Main(string[] args)
        {
            new CliProgram("TCPezy", typeof(HelpCommand), args).Run();
        }
    }
}
