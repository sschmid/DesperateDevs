using DesperateDevs.CLI.Utils;

namespace DesperateDevs.Networking.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            new CLIProgram("TCPezy", typeof(HelpCommand), args).Run();
        }
    }
}
