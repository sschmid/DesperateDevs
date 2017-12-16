using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DesperateDevs.Logging;

namespace DesperateDevs.Utils {

    public class AssemblyResolver {

        readonly Logger _logger = fabl.GetLogger(typeof(AssemblyResolver).Name);

        readonly AppDomain _appDomain;
        readonly string[] _basePaths;
        readonly List<Assembly> _assemblies;

        public AssemblyResolver(AppDomain appDomain, params string[] basePaths) {
            _appDomain = appDomain;
            _appDomain.AssemblyResolve += onAssemblyResolve;
            _basePaths = basePaths;
            _assemblies = new List<Assembly>();
        }

        public void Load(string path) {
            _logger.Debug("AppDomain load: " + path);
            var assembly = _appDomain.Load(path);
            _assemblies.Add(assembly);
        }

        Assembly onAssemblyResolve(object sender, ResolveEventArgs args) {
            Assembly assembly = null;
            try {
                _logger.Debug("  ➜ Loading: " + args.Name);
                assembly = Assembly.LoadFrom(args.Name);
            } catch (Exception) {
                var name = new AssemblyName(args.Name).Name;
                if (!name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) {
                    name += ".dll";
                }

                var path = resolvePath(name);
                if (path != null) {
                    assembly = Assembly.LoadFrom(path);
                }
            }

            return assembly;
        }

        string resolvePath(string assemblyName) {
            foreach (var basePath in _basePaths) {
                var path = basePath + Path.DirectorySeparatorChar + assemblyName;
                if (File.Exists(path)) {
                    _logger.Debug("    √ Resolved: " + path);
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
