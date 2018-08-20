using System;
using System.IO;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Logging;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class WizardCommand : AbstractCommand
    {
        public override string trigger { get { return "wiz"; } }
        public override string description { get { return "Setup Jenny, guided by a wizard"; } }
        public override string group { get { return null; } }
        public override string example { get { return "wiz"; } }

        readonly Logger _logger = fabl.GetLogger(typeof(WizardCommand));

        protected override void run()
        {
            const string title = @"
     gg
    dP8,
   dP Yb
  ,8  `8,
  I8   Yb
  `8b, `8,    ,ggg,    ,ggg,,ggg,    ,ggg,,ggg,   gg     gg
   `'Y88888  i8' '8i  ,8' '8P' '8,  ,8' '8P' '8,  I8     8I
       'Y8   I8, ,8I  I8   8I   8I  I8   8I   8I  I8,   ,8I
        ,88, `YbadP' ,dP   8I   Yb,,dP   8I   Yb,,d8b, ,d8I
    ,ad88888888P'Y8888P'   8I   `Y88P'   8I   `Y8P''Y88P'888
  ,dP''   Yb                                           ,d8I'
 ,8'      I8                                         ,dP'8I
,8'       I8                                        ,8'  8I
I8,      ,8'    A lovely .NET Code Generator        I8   8I
`Y8,___,d8'                                         `8, ,8I
  'Y888P'                                            `Y8P'
";

            var indent = "→ ";

            // Step 1: Properties
            var allPreferenes = Preferences.FindAll();
            var propertiesMenu = new Step1_PropertiesMenu(title, CLIHelper.consoleColors, allPreferenes);
            propertiesMenu.indent = indent;
            propertiesMenu.Start();

            var preferences = new Preferences(propertiesMenu.properties, null);

            // Step 2: Plugins
            var pluginsMenu = new Step2_PluginsMenu(title, CLIHelper.consoleColors, preferences);
            pluginsMenu.indent = indent;
            pluginsMenu.Start();

            var fixCommand = new FixCommand();
            fixCommand.Run(new[] { fixCommand.trigger, preferences.propertiesPath, "-s" });

            Console.Clear();
        }

        bool doesAlreadyExist(string path)
        {
            if (File.Exists(path))
            {
                _logger.Warn(path + " already exists!");
                _logger.Info("Use 'new -f' to overwrite the exiting file.");
                _logger.Info("Use 'edit' to open the exiting file.");

                return true;
            }

            return false;
        }
    }
}
