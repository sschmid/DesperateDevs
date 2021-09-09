using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class StatusCommand : AbstractPreferencesCommand
    {
        public override string trigger => "status";
        public override string description => "List available and unavailable plugins";
        public override string group => CommandGroups.PLUGINS;
        public override string example => "status";

        public StatusCommand() : base(typeof(StatusCommand).FullName)
        {
        }

        protected override void run()
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
                    config.defaultProperties.Keys.ToArray(),
                    _preferences);
                throw;
            }

            var requiredKeys = config.defaultProperties
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
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IPreProcessor>(instances, config.preProcessors));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IDataProvider>(instances, config.dataProviders));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<ICodeGenerator>(instances, config.codeGenerators));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IPostProcessor>(instances, config.postProcessors));

            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IPreProcessor>(instances, config.preProcessors));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IDataProvider>(instances, config.dataProviders));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<ICodeGenerator>(instances, config.codeGenerators));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IPostProcessor>(instances, config.postProcessors));
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
            printDuplicates(config.preProcessors);
            printDuplicates(config.dataProviders);
            printDuplicates(config.codeGenerators);
            printDuplicates(config.postProcessors);
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