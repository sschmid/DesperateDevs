using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Status : AbstractPreferencesCommand {

        public override string trigger { get { return "status"; } }
        public override string description { get { return "List available and unavailable plugins"; } }
        public override string example { get { return "jenny status"; } }

        public Status() : base(typeof(Status).Name) {
        }

        protected override void run() {
            var config = _preferences.CreateConfig<CodeGeneratorConfig>();
            var cliConfig = _preferences.CreateConfig<CLIConfig>();

            _logger.Debug(_preferences.ToString());

            Type[] types = null;
            Dictionary<string, string> defaultProperties = null;

            try {
                types = CodeGeneratorUtil.LoadTypesFromPlugins(_preferences);
                defaultProperties = CodeGeneratorUtil.GetDefaultProperties(types, config);
            } catch (Exception) {
                printKeyStatus(
                    config.defaultProperties.Merge(cliConfig.defaultProperties).Keys.ToArray(),
                    cliConfig,
                    _preferences);
                throw;
            }

            var requiredKeys = config.defaultProperties
                .Merge(cliConfig.defaultProperties)
                .Merge(defaultProperties).Keys.ToArray();

            printKeyStatus(requiredKeys, cliConfig, _preferences);
            printPluginStatus(types, config);
            printCollisions(config);
        }

        void printKeyStatus(string[] requiredKeys, CLIConfig cliConfig, Preferences preferences) {
            var unusedKeys = Helper
                .GetUnusedKeys(requiredKeys, preferences)
                .Where(key => !cliConfig.ignoreUnusedKeys.Contains(key));

            foreach (var key in unusedKeys) {
                _logger.Info("⚠️  Unused key: " + key);
            }

            foreach (var key in Helper.GetMissingKeys(requiredKeys, preferences)) {
                _logger.Warn("⚠️  Missing key: " + key);
            }
        }

        void printPluginStatus(Type[] types, CodeGeneratorConfig config) {
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IDataProvider>(types, config.dataProviders));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<ICodeGenerator>(types, config.codeGenerators));
            printUnavailable(CodeGeneratorUtil.GetUnavailableNamesOf<IPostProcessor>(types, config.postProcessors));

            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IDataProvider>(types, config.dataProviders));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<ICodeGenerator>(types, config.codeGenerators));
            printAvailable(CodeGeneratorUtil.GetAvailableNamesOf<IPostProcessor>(types, config.postProcessors));
        }

        void printUnavailable(string[] names) {
            foreach (var name in names) {
                _logger.Warn("⚠️ Unavailable: " + name);
            }
        }

        void printAvailable(string[] names) {
            foreach (var name in names) {
                _logger.Info("ℹ️  Available: " + name);
            }
        }

        void printCollisions(CodeGeneratorConfig config) {
            printDuplicates(config.dataProviders);
            printDuplicates(config.codeGenerators);
            printDuplicates(config.postProcessors);
        }

        void printDuplicates(string[] names) {
            var shortNames = names
                .Select(name => name.ShortTypeName())
                .ToArray();

            var duplicates = names
                .Where(name => shortNames.Count(n => n == name.ShortTypeName()) > 1)
                .OrderBy(name => name.ShortTypeName());

            foreach (var duplicate in duplicates) {
                _logger.Warn("⚠️  Potential collision detected: " + duplicate.ShortTypeName() + " -> " + duplicate);
            }
        }
    }
}
