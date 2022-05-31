using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DesperateDevs.Extensions;
using Sherlog;

namespace DesperateDevs.Reflection
{
    public class AssemblyResolver : IDisposable
    {
        readonly Logger _logger = Logger.GetLogger(nameof(AssemblyResolver));

        public IEnumerable<Assembly> Assemblies => _assemblies;

        readonly string[] _basePaths;
        readonly HashSet<Assembly> _assemblies;
        readonly AppDomain _appDomain;

        readonly ResolveEventHandler _cachedOnAssemblyResolve;

        public AssemblyResolver(params string[] basePaths)
        {
            _basePaths = basePaths;
            _assemblies = new HashSet<Assembly>();
            _appDomain = AppDomain.CurrentDomain;

            _cachedOnAssemblyResolve = OnAssemblyResolve;
            _appDomain.AssemblyResolve += _cachedOnAssemblyResolve;
        }

        public static AssemblyResolver LoadAssemblies(bool allDirectories, params string[] basePaths)
        {
            var resolver = new AssemblyResolver(basePaths);
            foreach (var file in GetAssemblyFiles(allDirectories, basePaths))
                resolver.Load(file);

            return resolver;
        }

        static IEnumerable<string> GetAssemblyFiles(bool allDirectories, params string[] basePaths)
        {
            var patterns = new[] {"*.dll", "*.exe"};
            var files = new List<string>();
            foreach (var pattern in patterns)
                files.AddRange(basePaths.SelectMany(s => Directory.GetFiles(s, pattern, allDirectories
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly)));

            return files;
        }

        public void Load(string path)
        {
            _logger.Debug($"{_appDomain} load: {path}");
            ResolveAndLoad(path, false);
        }

        Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) =>
            ResolveAndLoad(args.Name, true);

        Assembly ResolveAndLoad(string name, bool isDependency)
        {
            try
            {
                _logger.Debug(isDependency
                    ? $"  ➜ Loading dependency: {name}"
                    : $"  ➜ Loading: {name}");

                var assembly = Assembly.LoadFrom(name);
                _assemblies.Add(assembly);
                return assembly;
            }
            catch (Exception)
            {
                var path = ResolvePath(name);
                if (path != null)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(path);
                        _assemblies.Add(assembly);
                        return assembly;
                    }
                    catch (BadImageFormatException exception)
                    {
                        _logger.Warn(exception.Message);
                    }
                }
            }

            return null;
        }

        string ResolvePath(string name)
        {
            try
            {
                var assemblyName = new AssemblyName(name).Name;

                if (!assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                    !assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    assemblyName += ".dll";

                foreach (var basePath in _basePaths)
                {
                    var path = Path.Combine(basePath, assemblyName);
                    if (File.Exists(path))
                    {
                        _logger.Debug($"    ➜ Resolved: {path}");
                        return path;
                    }
                }
            }
            catch (FileLoadException)
            {
                _logger.Warn($"    × Could not resolve: {name}");
            }

            return null;
        }

        public IEnumerable<Type> GetTypes() => _assemblies.GetAllTypes();

        public void Dispose() => _appDomain.AssemblyResolve -= _cachedOnAssemblyResolve;
    }
}
