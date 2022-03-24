using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Logging;
using DesperateDevs.Serialization;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;

namespace DesperateDevs.CodeGeneration.CodeGenerator
{
    public static class CodeGeneratorUtil
    {
        static readonly DesperateDevs.Logging.Logger _logger = Sherlog.GetLogger(typeof(CodeGeneratorUtil).FullName);

        public static CodeGenerator CodeGeneratorFromPreferences(Preferences preferences)
        {
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

        static void configure(ICodeGenerationPlugin[] plugins, Preferences preferences)
        {
            foreach (var plugin in plugins.OfType<IConfigurable>())
                plugin.Configure(preferences);
        }

        public static ICodeGenerationPlugin[] LoadFromPlugins(Preferences preferences)
        {
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var resolver = new AssemblyResolver(false, config.searchPaths);
            foreach (var path in config.plugins)
                resolver.Load(path);

            return resolver.GetTypes()
                .GetNonAbstractTypes<ICodeGenerationPlugin>()
                .Select(type =>
                {
                    try
                    {
                        return (ICodeGenerationPlugin)Activator.CreateInstance(type);
                    }
                    catch (TypeLoadException ex)
                    {
                        _logger.Warn(ex.Message);
                    }

                    return null;
                })
                .Where(instance => instance != null)
                .ToArray();
        }

        public static T[] GetOrderedInstancesOf<T>(ICodeGenerationPlugin[] instances) where T : ICodeGenerationPlugin
        {
            return instances
                .OfType<T>()
                .OrderBy(instance => instance.priority)
                .ThenBy(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        public static string[] GetOrderedTypeNamesOf<T>(ICodeGenerationPlugin[] instances) where T : ICodeGenerationPlugin
        {
            return GetOrderedInstancesOf<T>(instances)
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        public static T[] GetEnabledInstancesOf<T>(ICodeGenerationPlugin[] instances, string[] typeNames) where T : ICodeGenerationPlugin
        {
            return GetOrderedInstancesOf<T>(instances)
                .Where(instance => typeNames.Contains(instance.GetType().ToCompilableString()))
                .ToArray();
        }

        public static string[] GetAvailableNamesOf<T>(ICodeGenerationPlugin[] instances, string[] typeNames) where T : ICodeGenerationPlugin
        {
            return GetOrderedTypeNamesOf<T>(instances)
                .Where(typeName => !typeNames.Contains(typeName))
                .ToArray();
        }

        public static string[] GetUnavailableNamesOf<T>(ICodeGenerationPlugin[] instances, string[] typeNames) where T : ICodeGenerationPlugin
        {
            var orderedTypeNames = GetOrderedTypeNamesOf<T>(instances);
            return typeNames
                .Where(typeName => !orderedTypeNames.Contains(typeName))
                .ToArray();
        }

        public static Dictionary<string, string> GetDefaultProperties(ICodeGenerationPlugin[] instances, CodeGeneratorConfig config)
        {
            return new Dictionary<string, string>().Merge(
                GetEnabledInstancesOf<IPreProcessor>(instances, config.preProcessors).OfType<IConfigurable>()
                    .Concat(GetEnabledInstancesOf<IDataProvider>(instances, config.dataProviders).OfType<IConfigurable>())
                    .Concat(GetEnabledInstancesOf<ICodeGenerator>(instances, config.codeGenerators).OfType<IConfigurable>())
                    .Concat(GetEnabledInstancesOf<IPostProcessor>(instances, config.postProcessors).OfType<IConfigurable>())
                    .Select(instance => instance.DefaultProperties)
                    .ToArray());
        }

        public static string[] BuildSearchPaths(string[] searchPaths, string[] additionalSearchPaths)
        {
            return searchPaths
                .Concat(additionalSearchPaths)
                .Where(Directory.Exists)
                .ToArray();
        }

        public static void AutoImport(CodeGeneratorConfig config, params string[] searchPaths)
        {
            var assemblyPaths = AssemblyResolver
                .GetAssembliesContainingType<ICodeGenerationPlugin>(true, searchPaths)
                .GetAllTypes()
                .GetNonAbstractTypes<ICodeGenerationPlugin>()
                .Select(type => type.Assembly)
                .Distinct()
                .Select(assembly => assembly.CodeBase.MakePathRelativeTo(Directory.GetCurrentDirectory()))
                .ToArray();

            var currentFullPaths = new HashSet<string>(config.searchPaths.Select(Path.GetFullPath));
            var newPaths = assemblyPaths
                .Select(Path.GetDirectoryName)
                .Where(path => !currentFullPaths.Contains(path));

            config.searchPaths = config.searchPaths
                .Concat(newPaths)
                .Distinct()
                .OrderBy(path => path)
                .ToArray();

            config.plugins = assemblyPaths
                .Select(Path.GetFileNameWithoutExtension)
                .Distinct()
                .OrderBy(plugin => plugin)
                .ToArray();
        }
    }
}
