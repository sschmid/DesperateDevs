using DesperateDevs.Cli.Utils;

namespace TCPeasy.Cli
{
    static class Program
    {
        public static void Main(string[] args)
        {
            new CliProgram("TCPeasy", typeof(HelpCommand), args).Run();
        }
    }
}
