using DesperateDevs.Cli.Utils;

namespace TCPeasy.Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            new CliProgram("TCPeasy", typeof(HelpCommand), args).Run();
        }
    }
}
