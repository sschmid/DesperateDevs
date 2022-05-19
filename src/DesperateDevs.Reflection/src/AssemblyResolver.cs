using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DesperateDevs.Extensions;
using Sherlog;

namespace DesperateDevs.Reflection
{
    public partial class AssemblyResolver
    {
        readonly Logger _logger = Logger.GetLogger(nameof(AssemblyResolver));

        public Assembly[] Assemblies => _assemblies.ToArray();

        readonly bool _reflectionOnly;
        readonly string[] _basePaths;
        readonly HashSet<Assembly> _assemblies;
        readonly AppDomain _appDomain;

        public AssemblyResolver(bool reflectionOnly, params string[] basePaths)
        {
            _reflectionOnly = reflectionOnly;
            _basePaths = basePaths;
            _assemblies = new HashSet<Assembly>();
            _appDomain = AppDomain.CurrentDomain;

            if (reflectionOnly)
                _appDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
            else
                _appDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public void Load(string path)
        {
            if (_reflectionOnly)
            {
                _logger.Debug(_appDomain + " reflect: " + path);
                ResolveAndLoad(path, Assembly.ReflectionOnlyLoadFrom, false);
            }
            else
            {
                _logger.Debug(_appDomain + " load: " + path);
                ResolveAndLoad(path, Assembly.LoadFrom, false);
            }
        }

        public void Close()
        {
            if (_reflectionOnly)
                _appDomain.ReflectionOnlyAssemblyResolve -= OnReflectionOnlyAssemblyResolve;
            else
                _appDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        Assembly ResolveAndLoad(string name, Func<string, Assembly> loadMethod, bool isDependency)
        {
            Assembly assembly = null;
            try
            {
                if (isDependency)
                    _logger.Debug("  ➜ Loading Dependency: " + name);
                else
                    _logger.Debug("  ➜ Loading: " + name);

                assembly = loadMethod(name);
                AddAssembly(assembly);
            }
            catch (Exception)
            {
                var path = ResolvePath(name);
                if (path != null)
                {
                    try
                    {
                        assembly = loadMethod(path);
                        AddAssembly(assembly);
                    }
                    catch (BadImageFormatException) { }
                }
            }

            return assembly;
        }

        Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) =>
            ResolveAndLoad(args.Name, Assembly.LoadFrom, true);

        Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args) =>
            ResolveAndLoad(args.Name, Assembly.ReflectionOnlyLoadFrom, true);

        void AddAssembly(Assembly assembly) => _assemblies.Add(assembly);

        string ResolvePath(string name)
        {
            try
            {
                var assemblyName = new AssemblyName(name).Name;

                if (!assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                    !assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    assemblyName += ".dll";
                }

                foreach (var basePath in _basePaths)
                {
                    var path = basePath + Path.DirectorySeparatorChar + assemblyName;
                    if (File.Exists(path))
                    {
                        _logger.Debug("    ➜ Resolved: " + path);
                        return path;
                    }
                }
            }
            catch (FileLoadException)
            {
                _logger.Debug("    × Could not resolve: " + name);
            }

            return null;
        }

        public Type[] GetTypes() => _assemblies.GetAllTypes().ToArray();
    }
}
