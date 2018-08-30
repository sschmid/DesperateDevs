using System;
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
            const string title = "";
//            const string title = @"
//     gg
//    dP8,
//   dP Yb
//  ,8  `8,
//  I8   Yb
//  `8b, `8,    ,ggg,    ,ggg,,ggg,    ,ggg,,ggg,   gg     gg
//   `'Y88888  i8' '8i  ,8' '8P' '8,  ,8' '8P' '8,  I8     8I
//       'Y8   I8, ,8I  I8   8I   8I  I8   8I   8I  I8,   ,8I
//        ,88, `YbadP' ,dP   8I   Yb,,dP   8I   Yb,,d8b, ,d8I
//    ,ad88888888P'Y8888P'   8I   `Y88P'   8I   `Y8P''Y88P'888
//  ,dP''   Yb                                           ,d8I'
// ,8'      I8                                         ,dP'8I
//,8'       I8                                        ,8'  8I
//I8,      ,8'    A lovely .NET Code Generator        I8   8I
//`Y8,___,d8'                                         `8, ,8I
//  'Y888P'                                            `Y8P'
//";

            const string indent = "→ ";

            // Step 1: Properties
            var allPreferenes = Preferences.FindAll();
            var propertiesMenu = new Step1_PropertiesMenu(_program, title, CLIHelper.consoleColors, allPreferenes);
            propertiesMenu.indent = indent;
            propertiesMenu.Start();

            var preferences = new Preferences(propertiesMenu.properties, null);

            // Step 2: Plugins
            var pluginsMenu = new Step2_PluginsMenu(_program, title, CLIHelper.consoleColors, preferences, _rawArgs.IsVerbose());
            pluginsMenu.indent = indent;
            pluginsMenu.Start();

            var fixArgs = pluginsMenu.shouldAutoImport
                ? "-s"
                : string.Empty;

            var fixCommand = new FixCommand();
            fixCommand.Run(_program, new[] { fixCommand.trigger, preferences.propertiesPath, fixArgs });

            Console.Clear();

            var doctorCommand = new DoctorCommand();
            doctorCommand.Run(_program, new[] { doctorCommand.trigger, preferences.propertiesPath });
        }
    }
}
