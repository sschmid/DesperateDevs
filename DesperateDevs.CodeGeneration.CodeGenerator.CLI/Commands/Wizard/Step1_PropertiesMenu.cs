using System;
using System.IO;
using System.Linq;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class Step1_PropertiesMenu : CLIMenu
    {
        public string properties;

        public Step1_PropertiesMenu(CLIProgram progam, string title, ConsoleColors colors, string[] properties) : base(buildTitle(title, properties), colors)
        {
            foreach (var p in properties)
                AddMenuEntry(new SelectPropertiesMenuEntry(this, p.MakePathRelativeTo(Directory.GetCurrentDirectory())));

            if (!properties.Any(p => p.EndsWith(Preferences.DEFAULT_PATH)))
                AddMenuEntry(new CreateDefaultPropertiesMenuEntry(progam, this));

            AddMenuEntry(new CreateCustomPropertiesMenuEntry(progam, this));
            AddMenuEntry(new ExitMenuEntry("Exit", false));
        }

        static string buildTitle(string title, string[] properties)
        {
            var header = title + "\n" +
                         "Step 1: Properties File\n" +
                         "=======================\n\n" +
                         "Jenny saves all its configuration in a *.properties file.\n";

            if (properties.Length == 0)
            {
                return header +
                       "No properties file was found in " + Directory.GetCurrentDirectory() + "\n" +
                       "Let's create this file now!\n\n" +
                       "Please choose how you want to proceed and press enter";
            }
            else
            {
                return header +
                       "These properties files were found in " + Directory.GetCurrentDirectory() + "\n" +
                       string.Join("\n", properties.Select(p => "- " + p.MakePathRelativeTo(Directory.GetCurrentDirectory()))) + "\n\n" +
                       "Please choose how you want to proceed and press enter";
            }
        }
    }

    public class SelectPropertiesMenuEntry : MenuEntry
    {
        public SelectPropertiesMenuEntry(Step1_PropertiesMenu menu, string properties) :
            base("Use " + properties, null, false, () =>
            {
                menu.properties = properties;
                menu.Stop();
            })
        {
        }
    }

    public class CreateDefaultPropertiesMenuEntry : MenuEntry
    {
        public CreateDefaultPropertiesMenuEntry(CLIProgram progam, Step1_PropertiesMenu menu) :
            base("Create new Jenny.properties", null, false, () =>
            {
                var command = new NewConfigCommand();
                command.Run(progam, new[] { command.trigger, Preferences.DEFAULT_PATH });
                menu.properties = Preferences.DEFAULT_PATH;
                menu.Stop();
            })
        {
        }
    }

    public class CreateCustomPropertiesMenuEntry : MenuEntry
    {
        public CreateCustomPropertiesMenuEntry(CLIProgram progam, Step1_PropertiesMenu menu) :
            base("Create a new properties file with a custom name", null, false, () =>
            {
                Console.WriteLine("Please enter a file name");
                var fileName = Console.ReadLine();
                var command = new NewConfigCommand();
                command.Run(progam, new[] { command.trigger, fileName });
                menu.properties = fileName;
                menu.Stop();
            })
        {
        }
    }
}
