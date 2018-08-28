using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class Step2_PluginsMenu : CLIMenu
    {
        public bool shouldAutoImport;

        public Step2_PluginsMenu(CLIProgram progam, string title, ConsoleColors colors, Preferences preferences) : base(buildTitle(title), colors)
        {
            Console.WriteLine(title);
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var allPlugins = autoImport(config);

            var pluginEntries = new List<SelectableMenuEntry>(allPlugins.Length);
            foreach (var plugin in allPlugins)
            {
                var localPlugin = plugin;
                var entry = new SelectableMenuEntry(localPlugin, config.plugins.Contains(localPlugin), isSelected => updateConfig(config, localPlugin, isSelected));
                AddMenuEntry(entry);
                pluginEntries.Add(entry);
            }

            if (allPlugins.Length > 0)
            {
                AddMenuEntry(new SelectableMenuEntry("Select all", pluginEntries.All(e => e.isSelected), isSelected =>
                {
                    foreach (var entry in pluginEntries)
                        entry.isSelected = isSelected;
                }));

                AddMenuEntry(new AutoSaveMenuEntry(this, preferences, config));
                AddMenuEntry(new ManualSaveMenuEntry(this, preferences, config));
            }
            else
            {
                AddMenuEntry(new EditMenuEntry(progam, this, preferences.propertiesPath));
            }

            AddMenuEntry(new ExitMenuEntry("Quit", false));
        }

        string[] autoImport(CodeGeneratorConfig config)
        {
            var selectedPlugins = config.plugins;
            var searchPaths = CodeGeneratorUtil.BuildSearchPaths(config.searchPaths, new[] { "." });
            var task = Task.Run(() => CodeGeneratorUtil.AutoImport(config, searchPaths));

            var spinner = new Spinner(SpinnerStyles.magicCat);
            spinner.Append("Searching for plugins. Please wait...");
            spinner.WriteWhile(0, Console.CursorTop, () => !task.IsCompleted);

            var allPlugins = config.plugins;
            config.plugins = selectedPlugins;
            return allPlugins;
        }

        void updateConfig(CodeGeneratorConfig config, string plugin, bool isSelected)
        {
            var list = config.plugins.ToList();
            if (isSelected)
                list.Add(plugin);
            else
                list.Remove(plugin);

            config.plugins = list
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
                   "- No plugins found: Make sure to specify the paths to plugins in " + CodeGeneratorConfig.SEARCH_PATHS_KEY +
                   "\n\n" +
                   "Please select the plugins you want to activate";
        }
    }

    public class AutoSaveMenuEntry : MenuEntry
    {
        public AutoSaveMenuEntry(Step2_PluginsMenu menu, Preferences preferences, CodeGeneratorConfig config) :
            base("Save and continue (auto import)", null, false, () =>
            {
                config.searchPaths = config.searchPaths
                    .OrderBy(path => path)
                    .ToArray();

                config.plugins = config.plugins
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
                config.searchPaths = config.searchPaths
                    .OrderBy(path => path)
                    .ToArray();

                config.plugins = config.plugins
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
