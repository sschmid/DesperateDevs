using System;
using DesperateDevs.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class HelpCommand : AbstractCommand
    {
        public override string trigger => "help";
        public override string description => "Show help";
        public override string group => null;
        public override string example => "help";

        protected override void run()
        {
            var pad = _program.GetCommandListPad();
            var commandList = _program.GetFormattedCommandList();

            var args = "[-v]".PadRight(pad) + "   - verbose output\n" +
                       "[-s]".PadRight(pad) + "   - silent (minimal output)";

            const string examples = "Jenny automatically detects *.properties and *.userproperties.\n" +
                                    "If you have multiple property files in one folder, please specify the desired file(s) along with the command.\n\n" +
                                    "EXAMPLE\n" +
                                    "  jenny new Jenny.properties\n" +
                                    "  jenny auto-import -s\n" +
                                    "  jenny doctor\n" +
                                    "  jenny edit\n" +
                                    "  jenny fix\n" +
                                    "  jenny gen\n" +
                                    "  jenny gen MyOther.properties\n" +
                                    "  jenny gen MyOther.properties username.userproperties";

            Console.WriteLine("usage:\n" + commandList + "\n" + args + "\n\n" + examples);
        }
    }
}
