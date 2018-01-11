using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DesperateDevs.Logging;

namespace DesperateDevs.Utils {

    public partial class AssemblyResolver {

        readonly Logger _logger = fabl.GetLogger(typeof(AssemblyResolver).Name);

        public Assembly[] assemblies { get { return _assemblies.ToArray(); } }

        readonly bool _reflectionOnly;
        readonly string[] _basePaths;
        readonly HashSet<Assembly> _assemblies;

        public AssemblyResolver(bool reflectionOnly, params string[] basePaths) {
            _reflectionOnly = reflectionOnly;
            if (reflectionOnly) {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += onReflectionOnlyAssemblyResolve;
            } else {
                AppDomain.CurrentDomain.AssemblyResolve += onAssemblyResolve;
            }

            _basePaths = basePaths;
            _assemblies = new HashSet<Assembly>();
        }

        public void Load(string path) {
            if (_reflectionOnly) {
                _logger.Debug(AppDomain.CurrentDomain + " reflect: " + path);
                resolveAndLoad(path, Assembly.ReflectionOnlyLoadFrom, false);
            } else {
                _logger.Debug(AppDomain.CurrentDomain + " load: " + path);
                resolveAndLoad(path, Assembly.LoadFrom, false);
            }
        }

        public void Close() {
            if (_reflectionOnly) {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= onReflectionOnlyAssemblyResolve;
            } else {
                AppDomain.CurrentDomain.AssemblyResolve -= onAssemblyResolve;
            }
        }

        Assembly resolveAndLoad(string name, Func<string, Assembly> loadMethod, bool isDependency) {
            Assembly assembly = null;
            try {
                if (isDependency) {
                    _logger.Debug("  ➜ Loading Dependency: " + name);
                } else {
                    _logger.Debug("  ➜ Loading: " + name);
                }
                assembly = loadMethod(name);
                addAssembly(assembly);
            } catch (Exception) {
                var path = resolvePath(name);
                if (path != null) {
                    try {
                        assembly = loadMethod(path);
                        addAssembly(assembly);
                    } catch (BadImageFormatException) {
                    }
                }
            }

            return assembly;
        }

        Assembly onAssemblyResolve(object sender, ResolveEventArgs args) {
            return resolveAndLoad(args.Name, Assembly.LoadFrom, true);
        }

        Assembly onReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args) {
            return resolveAndLoad(args.Name, Assembly.ReflectionOnlyLoadFrom, true);
        }

        void addAssembly(Assembly assembly) {
            _assemblies.Add(assembly);
        }

        string resolvePath(string name) {
            var assemblyName = new AssemblyName(name).Name;
            if (!assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                !assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) {
                assemblyName += ".dll";
            }

            foreach (var basePath in _basePaths) {
                var path = basePath + Path.DirectorySeparatorChar + assemblyName;
                if (File.Exists(path)) {
                    _logger.Debug("    ➜ Resolved: " + path);
                    return path;
                }
            }

            _logger.Warn("    × Could not resolve: " + assemblyName);

            return null;
        }

        public Type[] GetTypes() {
            return _assemblies.GetAllTypes();
        }
    }
}
