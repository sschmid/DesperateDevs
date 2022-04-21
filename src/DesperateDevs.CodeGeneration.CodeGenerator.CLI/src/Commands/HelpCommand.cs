﻿using System;
using DesperateDevs.Cli.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Cli
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

            var args = "[-v]".PadRight(pad) + "   - verbose output\n" +
                       "[-s]".PadRight(pad) + "   - silent (minimal output)";

            const string menu = "Menus\n\n" +
                                "  Down, Right, j, l          - Select next\n" +
                                "  Up, Left, k, h             - Select previous\n" +
                                "  Home, a                    - Select first\n" +
                                "  End, e                     - Select last\n" +
                                "  Enter, Space               - Run selected menu entry";

            var examples = "Jenny automatically uses " + CodeGenerator.DefaultPropertiesPath + " and <userName>.userproperties\n" +
                           "when no properties files are specified along with the command.\n\n" +
                           "EXAMPLE\n" +
                           "  jenny new Jenny.properties\n" +
                           "  jenny auto-import -s\n" +
                           "  jenny doctor\n" +
                           "  jenny edit\n" +
                           "  jenny fix\n" +
                           "  jenny gen\n" +
                           "  jenny gen Other.properties\n" +
                           "  jenny gen Other.properties Other.userproperties";

            Console.WriteLine("usage:\n" + commandList + "\n" + args + "\n\n" + menu + "\n\n" + examples);
        }
    }
}
