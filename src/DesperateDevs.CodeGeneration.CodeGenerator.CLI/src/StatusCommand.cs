using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.Cli.Utils;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Cli
{
    public class StatusCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "status";
        public override string Description => "List available and unavailable plugins";
        public override string Group => CommandGroups.PLUGINS;
        public override string Example => "status";

        public StatusCommand() : base(typeof(StatusCommand).FullName) { }

        protected override void Run()
        {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();

            _logger.Debug(_preferences.ToString());

            ICodeGenerationPlugin[] instances = null;
            Dictionary<string, string> defaultProperties = null;

            try
            {
                instances = CodeGeneratorUtil.LoadFromPlugins(_preferences);
                defaultProperties = CodeGeneratorUtil.GetDefaultProperties(instances, config);
            }
            catch (Exception)
            {
                printKeyStatus(
                    config.DefaultProperties.Keys.ToArray(),
                    _preferences);
                throw;
            }

            var requiredKeys = config.DefaultProperties
                .Merge(defaultProperties).Keys.ToArray();

            printKeyStatus(requiredKeys, _preferences);
            printPluginStatus(instances, config);
            printCollisions(config);
        }

        void printKeyStatus(string[] requiredKeys, Preferences preferences)
        {
            var unusedKeys = preferences.GetUnusedKeys(requiredKeys);
            foreach (var key in unusedKeys)
            {
                _logger.Info("ℹ️️  Unused key: " + key);
            }

            foreach (var key in preferences.GetMissingKeys(requiredKeys))
            {
                _logger.Warn("⚠️  Missing key: " + key);
            }
        }

        void printPluginStatus(ICodeGenerationPlugin[] instances, CodeGeneratorConfig config)
        {
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IPreProcessor>(instances, config.PreProcessors));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IDataProvider>(instances, config.DataProviders));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<ICodeGenerator>(instances, config.CodeGenerators));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IPostProcessor>(instances, config.PostProcessors));

            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IPreProcessor>(instances, config.PreProcessors));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IDataProvider>(instances, config.DataProviders));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<ICodeGenerator>(instances, config.CodeGenerators));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IPostProcessor>(instances, config.PostProcessors));
        }

        void printUnavailable(string[] names)
        {
            foreach (var name in names)
            {
                _logger.Warn("⚠️  Unavailable: " + name);
            }
        }

        void printAvailable(string[] names)
        {
            foreach (var name in names)
            {
                _logger.Info("ℹ️  Available: " + name);
            }
        }

        void printCollisions(CodeGeneratorConfig config)
        {
            printDuplicates(config.PreProcessors);
            printDuplicates(config.DataProviders);
            printDuplicates(config.CodeGenerators);
            printDuplicates(config.PostProcessors);
        }

        void printDuplicates(string[] names)
        {
            var shortNames = names
                .Select(name => name.ShortTypeName())
                .ToArray();

            var duplicates = names
                .Where(name => shortNames.Count(n => n == name.ShortTypeName()) > 1)
                .OrderBy(name => name.ShortTypeName());

            foreach (var duplicate in duplicates)
            {
                _logger.Warn("⚠️  Potential collision detected: " + duplicate.ShortTypeName() + " -> " + duplicate);
            }
        }
    }
}
