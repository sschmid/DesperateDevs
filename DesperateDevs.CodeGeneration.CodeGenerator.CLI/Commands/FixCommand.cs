using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class FixCommand : AbstractPreferencesCommand
    {
        public override string trigger => "fix";
        public override string description => "Add missing keys and add available or remove unavailable plugins interactively";
        public override string group => CommandGroups.PLUGINS;
        public override string example => "fix";

        static bool silent;

        public FixCommand() : base(typeof(FixCommand).FullName)
        {
        }

        protected override void run()
        {
            silent = _rawArgs.IsSilent();

            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();
            forceAddMissingKeys(config.defaultProperties, _preferences);

            var instances = CodeGeneratorUtil.LoadFromPlugins(_preferences);
            // A test to check if all types can be resolved and instantiated.
            CodeGeneratorUtil.GetEnabledInstancesOf<IPreProcessor>(instances, config.preProcessors);
            CodeGeneratorUtil.GetEnabledInstancesOf<IDataProvider>(instances, config.dataProviders);
            CodeGeneratorUtil.GetEnabledInstancesOf<ICodeGenerator>(instances, config.codeGenerators);
            CodeGeneratorUtil.GetEnabledInstancesOf<IPostProcessor>(instances, config.postProcessors);

            var askedRemoveKeys = new HashSet<string>();
            var askedAddKeys = new HashSet<string>();
            while (fix(askedRemoveKeys, askedAddKeys, instances, config, _preferences))
            {
            }

            runDoctors();
            fixSearchPath(instances, config, _preferences);
        }

        void runDoctors()
        {
            var doctors = AppDomain.CurrentDomain.GetInstancesOf<IDoctor>();
            foreach (var doctor in doctors.OfType<IConfigurable>())
            {
                doctor.Configure(_preferences);
            }

            foreach (var doctor in doctors)
            {
                var diagnosis = doctor.Diagnose();
                if (diagnosis.severity == DiagnosisSeverity.Error)
                {
                    if (silent)
                    {
                        if (doctor.Fix())
                        {
                            _preferences.Save();
                            _logger.Info("💉  Applied fix: " + diagnosis.treatment);
                        }
                    }
                    else
                    {
                        Console.WriteLine("💉  Apply fix: " + diagnosis.treatment);
                        Console.WriteLine("to treat symptoms: " + diagnosis.symptoms + " ? (y / n)");
                        if (PreferencesExtension.GetUserDecision())
                        {
                            if (doctor.Fix())
                            {
                                _preferences.Save();
                            }
                        }
                    }
                }
            }
        }

        void fixSearchPath(ICodeGenerationPlugin[] instances, CodeGeneratorConfig config, Preferences preferences)
        {
            var requiredSearchPaths = instances
                .Select(instance => Path.GetDirectoryName(instance.GetType().Assembly.CodeBase.MakePathRelativeTo(Directory.GetCurrentDirectory())))
                .Distinct()
                .Select(Path.GetFullPath)
                .OrderBy(path => path)
                .ToArray();

            var unusedPaths = config.searchPaths
                .Where(path => !requiredSearchPaths.Contains(Path.GetFullPath(path)));

            foreach (var path in unusedPaths)
            {
                if (silent)
                {
                    preferences.RemoveValue(path, config.searchPaths,
                        values => config.searchPaths = values);
                }
                else
                {
                    preferences.AskRemoveValue("Remove unused search path", path, config.searchPaths,
                        values => config.searchPaths = values);
                }
            }

            config.searchPaths = config.searchPaths.Distinct().ToArray();
            preferences.Save();
        }

        static void forceAddMissingKeys(Dictionary<string, string> requiredProperties, Preferences preferences)
        {
            var requiredKeys = requiredProperties.Keys.ToArray();
            var missingKeys = preferences.GetMissingKeys(requiredKeys);

            foreach (var key in missingKeys)
            {
                if (silent)
                    preferences.AddKey(key, requiredProperties[key]);
                else
                    preferences.NotifyForceAddKey("Will add missing key", key, requiredProperties[key]);
            }
        }

        bool fix(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, ICodeGenerationPlugin[] instances, CodeGeneratorConfig config, Preferences preferences)
        {
            var changed = fixPlugins(askedRemoveKeys, askedAddKeys, instances, config, preferences);
            changed |= fixCollisions(askedAddKeys, config, preferences);

            forceAddMissingKeys(CodeGeneratorUtil.GetDefaultProperties(instances, config), preferences);

            var requiredKeys = config.defaultProperties
                .Merge(CodeGeneratorUtil.GetDefaultProperties(instances, config))
                .Keys
                .ToArray();

            removeUnusedKeys(askedRemoveKeys, requiredKeys, preferences);

            return changed;
        }

        static bool fixPlugins(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, ICodeGenerationPlugin[] instances, CodeGeneratorConfig config, Preferences preferences)
        {
            var changed = false;

            var unavailablePreProcessors = CodeGeneratorUtil.GetUnavailableNamesOf<IPreProcessor>(instances, config.preProcessors);
            var unavailableDataProviders = CodeGeneratorUtil.GetUnavailableNamesOf<IDataProvider>(instances, config.dataProviders);
            var unavailableCodeGenerators = CodeGeneratorUtil.GetUnavailableNamesOf<ICodeGenerator>(instances, config.codeGenerators);
            var unavailablePostProcessors = CodeGeneratorUtil.GetUnavailableNamesOf<IPostProcessor>(instances, config.postProcessors);

            var availablePreProcessors = CodeGeneratorUtil.GetAvailableNamesOf<IPreProcessor>(instances, config.preProcessors);
            var availableDataProviders = CodeGeneratorUtil.GetAvailableNamesOf<IDataProvider>(instances, config.dataProviders);
            var availableCodeGenerators = CodeGeneratorUtil.GetAvailableNamesOf<ICodeGenerator>(instances, config.codeGenerators);
            var availablePostProcessors = CodeGeneratorUtil.GetAvailableNamesOf<IPostProcessor>(instances, config.postProcessors);

            foreach (var value in unavailablePreProcessors)
            {
                if (!askedRemoveKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.RemoveValue(value, config.preProcessors,
                            values => config.preProcessors = values);
                    }
                    else
                    {
                        preferences.AskRemoveValue("Remove unavailable pre processor", value, config.preProcessors,
                            values => config.preProcessors = values);
                    }
                    askedRemoveKeys.Add(value);
                    changed = true;
                }
            }

            foreach (var value in unavailableDataProviders)
            {
                if (!askedRemoveKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.RemoveValue(value, config.dataProviders,
                            values => config.dataProviders = values);
                    }
                    else
                    {
                        preferences.AskRemoveValue("Remove unavailable data provider", value, config.dataProviders,
                            values => config.dataProviders = values);
                    }

                    askedRemoveKeys.Add(value);
                    changed = true;
                }
            }

            foreach (var value in unavailableCodeGenerators)
            {
                if (!askedRemoveKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.RemoveValue(value, config.codeGenerators,
                            values => config.codeGenerators = values);
                    }
                    else
                    {
                        preferences.AskRemoveValue("Remove unavailable code generator", value, config.codeGenerators,
                            values => config.codeGenerators = values);
                    }

                    askedRemoveKeys.Add(value);
                    changed = true;
                }
            }

            foreach (var value in unavailablePostProcessors)
            {
                if (!askedRemoveKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.RemoveValue(value, config.postProcessors,
                            values => config.postProcessors = values);
                    }
                    else
                    {
                        preferences.AskRemoveValue("Remove unavailable post processor", value, config.postProcessors,
                            values => config.postProcessors = values);
                    }
                    askedRemoveKeys.Add(value);
                    changed = true;
                }
            }

            foreach (var value in availablePreProcessors)
            {
                if (!askedAddKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.AddValue(value, config.preProcessors,
                            values => config.preProcessors = values);
                    }
                    else
                    {
                        preferences.AskAddValue("Add available pre processor", value, config.preProcessors,
                            values => config.preProcessors = values);
                    }
                    askedAddKeys.Add(value);
                    changed = true;
                }
            }

            foreach (var value in availableDataProviders)
            {
                if (!askedAddKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.AddValue(value, config.dataProviders,
                            values => config.dataProviders = values);
                    }
                    else
                    {
                        preferences.AskAddValue("Add available data provider", value, config.dataProviders,
                            values => config.dataProviders = values);
                    }
                    askedAddKeys.Add(value);
                    changed = true;
                }
            }

            foreach (var value in availableCodeGenerators)
            {
                if (!askedAddKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.AddValue(value, config.codeGenerators,
                            values => config.codeGenerators = values);
                    }
                    else
                    {
                        preferences.AskAddValue("Add available code generator", value, config.codeGenerators,
                            values => config.codeGenerators = values);
                    }
                    askedAddKeys.Add(value);
                    changed = true;
                }
            }

            foreach (var value in availablePostProcessors)
            {
                if (!askedAddKeys.Contains(value))
                {
                    if (silent)
                    {
                        preferences.AddValue(value, config.postProcessors,
                            values => config.postProcessors = values);
                    }
                    else
                    {
                        preferences.AskAddValue("Add available post processor", value, config.postProcessors,
                            values => config.postProcessors = values);
                    }
                    askedAddKeys.Add(value);
                    changed = true;
                }
            }

            return changed;
        }

        bool fixCollisions(HashSet<string> askedAddKeys, CodeGeneratorConfig config, Preferences preferences)
        {
            var changed = fixDuplicates(askedAddKeys, config.preProcessors, values =>
            {
                config.preProcessors = values;
                return config.preProcessors;
            }, preferences);

            changed = fixDuplicates(askedAddKeys, config.dataProviders, values =>
            {
                config.dataProviders = values;
                return config.dataProviders;
            }, preferences) | changed;

            changed = fixDuplicates(askedAddKeys, config.codeGenerators, values =>
            {
                config.codeGenerators = values;
                return config.codeGenerators;
            }, preferences) | changed;

            return fixDuplicates(askedAddKeys, config.postProcessors, values =>
            {
                config.postProcessors = values;
                return config.postProcessors;
            }, preferences) | changed;
        }

        bool fixDuplicates(HashSet<string> askedAddKeys, string[] values, Func<string[], string[]> updateAction, Preferences preferences)
        {
            var changed = false;
            var duplicates = getDuplicates(values);

            foreach (var duplicate in duplicates)
            {
                Console.WriteLine("⚠️  Potential plugin collision: " + duplicate);
                Console.WriteLine("0: Keep all (no changes)");

                var collisions = values
                    .Where(name => name.EndsWith(duplicate))
                    .ToArray();

                printCollisions(collisions);
                var inputChars = getInputChars(collisions);
                var keyChar = PreferencesExtension.GetGenericUserDecision(inputChars);
                if (keyChar != '0')
                {
                    var index = int.Parse(keyChar.ToString()) - 1;
                    var keep = collisions[index];

                    foreach (var collision in collisions)
                    {
                        if (collision != keep)
                        {
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

        static string[] getDuplicates(string[] values)
        {
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

        void printCollisions(string[] collisions)
        {
            for (int i = 0; i < collisions.Length; i++)
            {
                Console.WriteLine((i + 1) + ": Keep " + collisions[i]);
            }
        }

        static char[] getInputChars(string[] collisions)
        {
            var chars = new char[collisions.Length + 1];
            for (int i = 0; i < collisions.Length; i++)
            {
                chars[i] = (i + 1).ToString()[0];
            }
            chars[chars.Length - 1] = '0';
            return chars;
        }

        static void removeUnusedKeys(HashSet<string> askedRemoveKeys, string[] requiredKeys, Preferences preferences)
        {
            var unusedKeys = preferences.GetUnusedKeys(requiredKeys);
            foreach (var key in unusedKeys)
            {
                if (!askedRemoveKeys.Contains(key))
                {
                    if (silent)
                    {
                        preferences.RemoveKey(key);
                    }
                    else
                    {
                        preferences.AskRemoveKey("Remove unused key", key);
                    }
                    askedRemoveKeys.Add(key);
                }
            }
        }
    }
}
