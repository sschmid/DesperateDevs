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
            const string title = @" --------------------------
|                          |
|      Jenny wizard        |
|                          |
|     ∧＿∧                 |
|   （｡･ω･｡)つ━☆・*。      |
|   ⊂　　 ノ 　　　・゜+.  |
|   しーＪ　　　°。+ *´¨)  |
|                          |
 --------------------------
";

            var indent = "→ ";
            var colors = new DefaultMenuColors();

            // Step 1: Properties
            var allPreferenes = Preferences.FindAll();
            var propertiesMenu = new Step1_PropertiesMenu(title, colors, allPreferenes);
            propertiesMenu.indent = indent;
            propertiesMenu.Start();

            var preferences = new Preferences(propertiesMenu.properties, null);

            // Step 2: Plugins
            var pluginsMenu = new Step2_PluginsMenu(title, colors, preferences);
            pluginsMenu.indent = indent;
            pluginsMenu.Start();

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
