using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator {

    public static class CodeGeneratorUtil {

        public static CodeGenerator CodeGeneratorFromPreferences(Preferences preferences) {
            var instances = LoadFromPlugins(preferences);
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();

            var preProcessors = GetEnabledInstancesOf<IPreProcessor>(instances, config.preProcessors);
            var dataProviders = GetEnabledInstancesOf<IDataProvider>(instances, config.dataProviders);
            var codeGenerators = GetEnabledInstancesOf<ICodeGenerator>(instances, config.codeGenerators);
            var postProcessors = GetEnabledInstancesOf<IPostProcessor>(instances, config.postProcessors);

            configure(preProcessors, preferences);
            configure(dataProviders, preferences);
            configure(codeGenerators, preferences);
            configure(postProcessors, preferences);

            return new CodeGenerator(preProcessors, dataProviders, codeGenerators, postProcessors);
        }

        static void configure(ICodeGenerationPlugin[] plugins, Preferences preferences) {
            foreach (var plugin in plugins.OfType<IConfigurable>()) {
                plugin.Configure(preferences);
            }
        }

        public static ICodeGenerationPlugin[] LoadFromPlugins(Preferences preferences) {
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var resolver = new AssemblyResolver(false, config.searchPaths);
            foreach (var path in config.plugins) {
                resolver.Load(path);
            }

            var instances = resolver
                .GetTypes()
                .GetInstancesOf<ICodeGenerationPlugin>();

            resolver.Close();

            return instances;
        }

        public static T[] GetOrderedInstancesOf<T>(ICodeGenerationPlugin[] instances) where T : ICodeGenerationPlugin {
            return instances
                .OfType<T>()
                .OrderBy(instance => instance.priority)
                .ThenBy(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        public static string[] GetOrderedTypeNamesOf<T>(ICodeGenerationPlugin[] instances) where T : ICodeGenerationPlugin {
            return GetOrderedInstancesOf<T>(instances)
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        public static T[] GetEnabledInstancesOf<T>(ICodeGenerationPlugin[] instances, string[] typeNames) where T : ICodeGenerationPlugin {
            return GetOrderedInstancesOf<T>(instances)
                .Where(instance => typeNames.Contains(instance.GetType().ToCompilableString()))
                .ToArray();
        }

        public static string[] GetAvailableNamesOf<T>(ICodeGenerationPlugin[] instances, string[] typeNames) where T : ICodeGenerationPlugin {
            return GetOrderedTypeNamesOf<T>(instances)
                .Where(typeName => !typeNames.Contains(typeName))
                .ToArray();
        }

        public static string[] GetUnavailableNamesOf<T>(ICodeGenerationPlugin[] instances, string[] typeNames) where T : ICodeGenerationPlugin {
            var orderedTypeNames = GetOrderedTypeNamesOf<T>(instances);
            return typeNames
                .Where(typeName => !orderedTypeNames.Contains(typeName))
                .ToArray();
        }

        public static Dictionary<string, string> GetDefaultProperties(ICodeGenerationPlugin[] instances, CodeGeneratorConfig config) {
            return new Dictionary<string, string>().Merge(
                GetEnabledInstancesOf<IPreProcessor>(instances, config.preProcessors).OfType<IConfigurable>()
                    .Concat(GetEnabledInstancesOf<IDataProvider>(instances, config.dataProviders).OfType<IConfigurable>())
                    .Concat(GetEnabledInstancesOf<ICodeGenerator>(instances, config.codeGenerators).OfType<IConfigurable>())
                    .Concat(GetEnabledInstancesOf<IPostProcessor>(instances, config.postProcessors).OfType<IConfigurable>())
                    .Select(instance => instance.defaultProperties)
                    .ToArray());
        }

        public static void AutoImport(CodeGeneratorConfig config, params string[] searchPaths) {
            var assemblies = AssemblyResolver
                .GetAssembliesContainingType<ICodeGenerationPlugin>(true, searchPaths)
                .GetAllTypes()
                .GetNonAbstractTypes<ICodeGenerationPlugin>()
                .Select(type => type.Assembly)
                .Distinct()
                .Select(assembly => new Uri(assembly.CodeBase))
                .Select(uri => uri.AbsolutePath + uri.Fragment)
                .Select(path => path.Replace(Directory.GetCurrentDirectory(), string.Empty))
                .Select(path => path.StartsWith(Path.DirectorySeparatorChar.ToString()) ? "." + path : path)
                .ToArray();

            config.searchPaths = config.searchPaths
                .Concat(assemblies.Select(Path.GetDirectoryName))
                .Distinct()
                .ToArray();

            config.plugins = assemblies
                .Select(Path.GetFileNameWithoutExtension)
                .Distinct()
                .ToArray();
        }
    }
}
