﻿using DesperateDevs.Cli.Utils;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class EditMenuEntry : MenuEntry
    {
        public EditMenuEntry(CliProgram progam, CliMenu menu, string propertiesPath) :
            base("Edit " + propertiesPath, null, false, () =>
            {
                var command = new EditConfigCommand();
                command.Run(progam, new[] { command.Trigger, propertiesPath });
                menu.Stop();
            })
        {
        }
    }
}
