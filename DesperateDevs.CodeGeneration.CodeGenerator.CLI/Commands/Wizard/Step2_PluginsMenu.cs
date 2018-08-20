using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class Step2_PluginsMenu : CLIMenu
    {
        public Step2_PluginsMenu(string title, ConsoleColors colors, Preferences preferences) : base(buildTitle(title), colors)
        {
            Console.WriteLine(title);
            Console.WriteLine("Searching for plugins. Please wait...");
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

            AddMenuEntry(new SelectableMenuEntry("Select all", pluginEntries.All(e => e.isSelected), isSelected =>
            {
                foreach (var entry in pluginEntries)
                    entry.isSelected = isSelected;
            }));
            AddMenuEntry(new SaveMenuEntry(this, preferences, config));
            AddMenuEntry(new ExitMenuEntry("Exit", false));
        }

        string[] autoImport(CodeGeneratorConfig config)
        {
            var selectedPlugins = config.plugins;
            var searchPaths = CodeGeneratorUtil.BuildSearchPaths(config.searchPaths, new[] { "." });
            CodeGeneratorUtil.AutoImport(config, searchPaths);

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
                   "Please select the plugins you want to activate";
        }
    }

    public class SaveMenuEntry : MenuEntry
    {
        public SaveMenuEntry(Step2_PluginsMenu menu, Preferences preferences, CodeGeneratorConfig config) :
            base("Save and continue", null, false, () =>
            {
                config.searchPaths = config.searchPaths
                    .OrderBy(path => path)
                    .ToArray();

                config.plugins = config.plugins
                    .OrderBy(path => path)
                    .ToArray();

                preferences.Save();
                menu.Stop();
            })
        {
        }
    }
}
