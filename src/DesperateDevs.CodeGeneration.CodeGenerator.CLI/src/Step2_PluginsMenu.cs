using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesperateDevs.Cli.Utils;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Cli
{
    public class Step2_PluginsMenu : CliMenu
    {
        public bool shouldAutoImport;

        readonly bool _isVerbose;

        public Step2_PluginsMenu(CliProgram progam, string title, Preferences preferences, bool isVerbose) : base(buildTitle(title))
        {
            _isVerbose = isVerbose;
            Console.WriteLine(title);
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var allPlugins = autoImport(config);

            var pluginEntries = new List<SelectableMenuEntry>(allPlugins.Length);

            if (allPlugins.Length > 0)
            {
                AddMenuEntry(new AutoSaveMenuEntry(this, preferences, config));
                AddMenuEntry(new ManualSaveMenuEntry(this, preferences, config));

                AddMenuEntry(new SelectableMenuEntry("Select all", pluginEntries.All(e => e.IsSelected), isSelected =>
                {
                    foreach (var entry in pluginEntries)
                        entry.IsSelected = isSelected;
                }));
            }
            else
            {
                AddMenuEntry(new EditMenuEntry(progam, this, preferences.PropertiesPath));
            }

            foreach (var plugin in allPlugins)
            {
                var localPlugin = plugin;
                var entry = new SelectableMenuEntry(localPlugin, config.Plugins.Contains(localPlugin), isSelected => updateConfig(config, localPlugin, isSelected));
                AddMenuEntry(entry);
                pluginEntries.Add(entry);
            }

            AddMenuEntry(new ExitMenuEntry("Quit", false));
        }

        string[] autoImport(CodeGeneratorConfig config)
        {
            var selectedPlugins = config.Plugins;
            var searchPaths = CodeGeneratorUtil.BuildSearchPaths(config.SearchPaths, new[] { "." });
            var task = Task.Run(() => CodeGeneratorUtil.AutoImport(config, searchPaths));

            if (!_isVerbose)
            {
                var top = Console.CursorTop;
                var spinner = new Spinner(SpinnerStyles.MagicCat);
                spinner.Append("Searching for plugins. Please wait...");
                spinner.WriteWhile(0, top, () => !task.IsCompleted);
            }
            else
            {
                while (!task.IsCompleted)
                {
                }
            }

            var allPlugins = config.Plugins;
            config.Plugins = selectedPlugins;
            return allPlugins;
        }

        void updateConfig(CodeGeneratorConfig config, string plugin, bool isSelected)
        {
            var list = config.Plugins.ToList();
            if (isSelected)
                list.Add(plugin);
            else
                list.Remove(plugin);

            config.Plugins = list
                .Distinct()
                .OrderBy(p => p)
                .ToArray();
        }

        static string buildTitle(string title)
        {
            return title + "\n" +
                   "Step 2: Plugins\n" +
                   "===============\n\n" +
                   "Plugins can contain one or more\n" +
                   "- PreProcessors - prepare the data source if needed\n" +
                   "- DataProviders - process the data source and create the model\n" +
                   "- CodeGenerators - read the model and generate CodeGenFiles in memory\n" +
                   "- PostProcessors - process the CodeGenFiles, e.g. writing to disc\n" +
                   "- Doctors - diagnose and fix problems\n\n" +
                   "FAQ:\n" +
                   "- No plugins found: Make sure to specify the paths to plugins in " + CodeGeneratorConfig.SearchPathsKey +
                   "\n\n" +
                   "Please select the plugins you want to activate";
        }
    }

    public class AutoSaveMenuEntry : MenuEntry
    {
        public AutoSaveMenuEntry(Step2_PluginsMenu menu, Preferences preferences, CodeGeneratorConfig config) :
            base("Save and continue (auto import)", null, false, () =>
            {
                config.SearchPaths = config.SearchPaths
                    .OrderBy(path => path)
                    .ToArray();

                config.Plugins = config.Plugins
                    .OrderBy(path => path)
                    .ToArray();

                preferences.Save();
                menu.shouldAutoImport = true;
                menu.Stop();
            })
        {
        }
    }

    public class ManualSaveMenuEntry : MenuEntry
    {
        public ManualSaveMenuEntry(Step2_PluginsMenu menu, Preferences preferences, CodeGeneratorConfig config) :
            base("Save and continue (manual import)", null, false, () =>
            {
                config.SearchPaths = config.SearchPaths
                    .OrderBy(path => path)
                    .ToArray();

                config.Plugins = config.Plugins
                    .OrderBy(path => path)
                    .ToArray();

                preferences.Save();
                menu.shouldAutoImport = false;
                menu.Stop();
            })
        {
        }
    }
}
