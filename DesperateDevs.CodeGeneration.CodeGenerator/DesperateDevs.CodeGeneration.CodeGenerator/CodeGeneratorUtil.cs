using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static CodeGenerator CodeGeneratorFromPreferences(Preferences preferences) {
            var types = LoadTypesFromPlugins(preferences);
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();

            var preProcessors = GetEnabledInstancesOf<IPreProcessor>(types, config.preProcessors);
            var dataProviders = GetEnabledInstancesOf<IDataProvider>(types, config.dataProviders);
            var codeGenerators = GetEnabledInstancesOf<ICodeGenerator>(types, config.codeGenerators);
            var postProcessors = GetEnabledInstancesOf<IPostProcessor>(types, config.postProcessors);

            configure(preProcessors, preferences);
            configure(dataProviders, preferences);
            configure(codeGenerators, preferences);
            configure(postProcessors, preferences);

            return new CodeGenerator(preProcessors, dataProviders, codeGenerators, postProcessors);
        }

        static void configure(ICodeGeneratorBase[] plugins, Preferences preferences) {
            foreach (var plugin in plugins.OfType<IConfigurable>()) {
                plugin.Configure(preferences);
            }
        }

        public static Type[] LoadTypesFromPlugins(Preferences preferences) {
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var resolver = new AssemblyResolver(AppDomain.CurrentDomain, config.searchPaths);
            foreach (var path in config.plugins) {
                resolver.Load(path);
            }

            return resolver.GetTypes();
        }

        public static T[] GetOrderedInstancesOf<T>(Type[] types) where T : ICodeGeneratorBase {
            return types
                .GetInstancesOf<T>()
                .OrderBy(instance => instance.priority)
                .ThenBy(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        public static string[] GetOrderedTypeNamesOf<T>(Type[] types) where T : ICodeGeneratorBase {
            return GetOrderedInstancesOf<T>(types)
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        public static T[] GetEnabledInstancesOf<T>(Type[] types, string[] typeNames) where T : ICodeGeneratorBase {
            return GetOrderedInstancesOf<T>(types)
                .Where(instance => typeNames.Contains(instance.GetType().ToCompilableString()))
                .ToArray();
        }

        public static string[] GetAvailableNamesOf<T>(Type[] types, string[] typeNames) where T : ICodeGeneratorBase {
            return GetOrderedTypeNamesOf<T>(types)
                .Where(typeName => !typeNames.Contains(typeName))
                .ToArray();
        }

        public static string[] GetUnavailableNamesOf<T>(Type[] types, string[] typeNames) where T : ICodeGeneratorBase {
            var orderedTypeNames = GetOrderedTypeNamesOf<T>(types);
            return typeNames
                .Where(typeName => !orderedTypeNames.Contains(typeName))
                .ToArray();
        }

        public static Dictionary<string, string> GetDefaultProperties(Type[] types, CodeGeneratorConfig config) {
            return new Dictionary<string, string>().Merge(
                GetEnabledInstancesOf<IPreProcessor>(types, config.preProcessors).OfType<IConfigurable>()
                    .Concat(GetEnabledInstancesOf<IDataProvider>(types, config.dataProviders).OfType<IConfigurable>())
                    .Concat(GetEnabledInstancesOf<ICodeGenerator>(types, config.codeGenerators).OfType<IConfigurable>())
                    .Concat(GetEnabledInstancesOf<IPostProcessor>(types, config.postProcessors).OfType<IConfigurable>())
                    .Select(instance => instance.defaultProperties)
                    .ToArray());
        }
    }
}
