using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class FixConfig : AbstractPreferencesCommand {

        public override string trigger { get { return "fix"; } }
        public override string description { get { return "Add missing or remove unused keys interactively"; } }
        public override string example { get { return "fix"; } }

        public FixConfig() : base(typeof(FixConfig).FullName) {
        }

        protected override void run() {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var cliConfig = _preferences.CreateAndConfigure<CLIConfig>();

            forceAddMissingKeys(config.defaultProperties, _preferences);
            forceAddMissingKeys(cliConfig.defaultProperties, _preferences);

            var instances = CodeGeneratorUtil.LoadFromPlugins(_preferences);
            // A test to check if all types can be resolved and instantiated.
            CodeGeneratorUtil.GetEnabledInstancesOf<IPreProcessor>(instances, config.preProcessors);
            CodeGeneratorUtil.GetEnabledInstancesOf<IDataProvider>(instances, config.dataProviders);
            CodeGeneratorUtil.GetEnabledInstancesOf<ICodeGenerator>(instances, config.codeGenerators);
            CodeGeneratorUtil.GetEnabledInstancesOf<IPostProcessor>(instances, config.postProcessors);

            var askedRemoveKeys = new HashSet<string>();
            var askedAddKeys = new HashSet<string>();
            while (fix(askedRemoveKeys, askedAddKeys, instances, config, cliConfig, _preferences)) {
            }

            var doctors = AppDomain.CurrentDomain.GetInstancesOf<IDoctor>();
            foreach (var doctor in doctors.OfType<IConfigurable>()) {
                doctor.Configure(_preferences);
            }

            foreach (var doctor in doctors) {
                var diagnosis = doctor.Diagnose();
                if (diagnosis.severity == DiagnosisSeverity.Error) {
                    _logger.Info("💉  Apply fix: " + diagnosis.treatment);
                    _logger.Info("to treat symptoms: " + diagnosis.symptoms + " ? (y / n)");
                    if (PreferencesExtension.GetUserDecision()) {
                        if (doctor.Fix()) {
                            _preferences.Save();
                        }
                    }
                }
            }
        }

        static void forceAddMissingKeys(Dictionary<string, string> requiredProperties, Preferences preferences) {
            var requiredKeys = requiredProperties.Keys.ToArray();
            var missingKeys = preferences.GetMissingKeys(requiredKeys);

            foreach (var key in missingKeys) {
                preferences.ForceAddKey("Will add missing key", key, requiredProperties[key]);
            }
        }

        bool fix(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, ICodeGeneratorBase[] instances, CodeGeneratorConfig config, CLIConfig cliConfig, Preferences preferences) {
            var changed = fixPlugins(askedRemoveKeys, askedAddKeys, instances, config, preferences);
            changed |= fixCollisions(askedAddKeys, config, preferences);

            forceAddMissingKeys(CodeGeneratorUtil.GetDefaultProperties(instances, config), preferences);

            var requiredKeys = config.defaultProperties
                .Merge(cliConfig.defaultProperties)
                .Merge(CodeGeneratorUtil.GetDefaultProperties(instances, config))
                .Keys
                .ToArray();

            removeUnusedKeys(askedRemoveKeys, requiredKeys, cliConfig, preferences);

            return changed;
        }

        static bool fixPlugins(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, ICodeGeneratorBase[] instances, CodeGeneratorConfig config, Preferences preferences) {
            var changed = false;

            var unavailablePreProcessors = CodeGeneratorUtil.GetUnavailableNamesOf<IPreProcessor>(instances, config.preProcessors);
            var unavailableDataProviders = CodeGeneratorUtil.GetUnavailableNamesOf<IDataProvider>(instances, config.dataProviders);
            var unavailableCodeGenerators = CodeGeneratorUtil.GetUnavailableNamesOf<ICodeGenerator>(instances, config.codeGenerators);
            var unavailablePostProcessors = CodeGeneratorUtil.GetUnavailableNamesOf<IPostProcessor>(instances, config.postProcessors);

            var availablePreProcessors = CodeGeneratorUtil.GetAvailableNamesOf<IPreProcessor>(instances, config.preProcessors);
            var availableDataProviders = CodeGeneratorUtil.GetAvailableNamesOf<IDataProvider>(instances, config.dataProviders);
            var availableCodeGenerators = CodeGeneratorUtil.GetAvailableNamesOf<ICodeGenerator>(instances, config.codeGenerators);
            var availablePostProcessors = CodeGeneratorUtil.GetAvailableNamesOf<IPostProcessor>(instances, config.postProcessors);

            foreach (var key in unavailablePreProcessors) {
                if (!askedRemoveKeys.Contains(key)) {
                    preferences.AskRemoveValue("Remove unavailable pre processor", key, config.preProcessors,
                        values => config.preProcessors = values);
                    askedRemoveKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in unavailableDataProviders) {
                if (!askedRemoveKeys.Contains(key)) {
                    preferences.AskRemoveValue("Remove unavailable data provider", key, config.dataProviders,
                        values => config.dataProviders = values);
                    askedRemoveKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in unavailableCodeGenerators) {
                if (!askedRemoveKeys.Contains(key)) {
                    preferences.AskRemoveValue("Remove unavailable code generator", key, config.codeGenerators,
                        values => config.codeGenerators = values);
                    askedRemoveKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in unavailablePostProcessors) {
                if (!askedRemoveKeys.Contains(key)) {
                    preferences.AskRemoveValue("Remove unavailable post processor", key, config.postProcessors,
                        values => config.postProcessors = values);
                    askedRemoveKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in availablePreProcessors) {
                if (!askedAddKeys.Contains(key)) {
                    preferences.AskAddValue("Add available pre processor", key, config.preProcessors,
                        values => config.preProcessors = values);
                    askedAddKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in availableDataProviders) {
                if (!askedAddKeys.Contains(key)) {
                    preferences.AskAddValue("Add available data provider", key, config.dataProviders,
                        values => config.dataProviders = values);
                    askedAddKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in availableCodeGenerators) {
                if (!askedAddKeys.Contains(key)) {
                    preferences.AskAddValue("Add available code generator", key, config.codeGenerators,
                        values => config.codeGenerators = values);
                    askedAddKeys.Add(key);
                    changed = true;
                }
            }

            foreach (var key in availablePostProcessors) {
                if (!askedAddKeys.Contains(key)) {
                    preferences.AskAddValue("Add available post processor", key, config.postProcessors,
                        values => config.postProcessors = values);
                    askedAddKeys.Add(key);
                    changed = true;
                }
            }

            return changed;
        }

        bool fixCollisions(HashSet<string> askedAddKeys, CodeGeneratorConfig config, Preferences preferences) {
            var changed = fixDuplicates(askedAddKeys, config.preProcessors, values => {
                config.preProcessors = values;
                return config.preProcessors;
            }, preferences);

            changed = fixDuplicates(askedAddKeys, config.dataProviders, values => {
                config.dataProviders = values;
                return config.dataProviders;
            }, preferences) | changed;

            changed = fixDuplicates(askedAddKeys, config.codeGenerators, values => {
                config.codeGenerators = values;
                return config.codeGenerators;
            }, preferences) | changed;

            return fixDuplicates(askedAddKeys, config.postProcessors, values => {
                config.postProcessors = values;
                return config.postProcessors;
            }, preferences) | changed;
        }

        bool fixDuplicates(HashSet<string> askedAddKeys, string[] values, Func<string[], string[]> updateAction, Preferences preferences) {
            var changed = false;
            var duplicates = getDuplicates(values);

            foreach (var duplicate in duplicates) {
                _logger.Info("⚠️  Potential plugin collision: " + duplicate);
                _logger.Info("0: Keep all (no changes)");

                var collisions = values
                    .Where(name => name.EndsWith(duplicate))
                    .ToArray();

                printCollisions(collisions);
                var inputChars = getInputChars(collisions);
                var keyChar = PreferencesExtension.GetGenericUserDecision(inputChars);
                if (keyChar != '0') {
                    var index = int.Parse(keyChar.ToString()) - 1;
                    var keep = collisions[index];

                    foreach (var collision in collisions) {
                        if (collision != keep) {
                            preferences.RemoveValue(
                                collision,
                                values,
                                result => values = updateAction(result));
                            askedAddKeys.Add(collision);
                            changed = true;
                        }
                    }
                }
            }

            return changed;
        }

        static string[] getDuplicates(string[] values) {
            var shortNames = values
                .Select(name => name.ShortTypeName())
                .ToArray();

            return values
                .Where(name => shortNames.Count(n => n == name.ShortTypeName()) > 1)
                .Select(name => name.ShortTypeName())
                .Distinct()
                .OrderBy(name => name.ShortTypeName())
                .ToArray();
        }

        void printCollisions(string[] collisions) {
            for (int i = 0; i < collisions.Length; i++) {
                _logger.Info((i + 1) + ": Keep " + collisions[i]);
            }
        }

        static char[] getInputChars(string[] collisions) {
            var chars = new char[collisions.Length + 1];
            for (int i = 0; i < collisions.Length; i++) {
                chars[i] = (i + 1).ToString()[0];
            }
            chars[chars.Length - 1] = '0';
            return chars;
        }

        static void removeUnusedKeys(HashSet<string> askedRemoveKeys, string[] requiredKeys, CLIConfig cliConfig, Preferences preferences) {
            var unusedKeys = preferences.GetUnusedKeys(requiredKeys)
                .Where(key => !cliConfig.ignoreUnusedKeys.Contains(key));

            foreach (var key in unusedKeys) {
                if (!askedRemoveKeys.Contains(key)) {
                    preferences.AskRemoveOrIgnoreKey("Remove unused key", key, cliConfig.ignoreUnusedKeys,
                        values => cliConfig.ignoreUnusedKeys = values);
                    askedRemoveKeys.Add(key);
                }
            }
        }
    }
}
