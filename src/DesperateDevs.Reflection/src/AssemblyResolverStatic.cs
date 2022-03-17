using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DesperateDevs.Reflection {

    public partial class AssemblyResolver {

        public static AssemblyResolver LoadAssemblies(bool allDirectories, params string[] basePaths) {
            var resolver = new AssemblyResolver(false, basePaths);
            foreach (var file in getAssemblyFiles(allDirectories, basePaths)) {
                resolver.Load(file);
            }

            return resolver;
        }

        public static Assembly[] GetAssembliesContainingType<T>(bool allDirectories, params string[] basePaths) {
            var resolver = new AssemblyResolver(true, basePaths);
            foreach (var file in getAssemblyFiles(allDirectories, basePaths)) {
                resolver.Load(file);
            }

            var interfaceName = typeof(T).FullName;
            var assemblies = resolver
                .GetTypes()
                .Where(type => type.GetInterface(interfaceName) != null)
                .Select(type => type.Assembly)
                .Distinct()
                .ToArray();

            resolver.Close();

            return assemblies;
        }

        public static AssemblyResolver LoadAssembliesContainingType<T>(bool allDirectories, params string[] basePaths) {
            var assemblies = GetAssembliesContainingType<T>(allDirectories, basePaths);

            var resolver = new AssemblyResolver(false, basePaths);
            foreach (var assembly in assemblies) {
                resolver.Load(assembly.CodeBase);
            }

            return resolver;
        }

        static string[] getAssemblyFiles(bool allDirectories, params string[] basePaths) {
            var patterns = new[] { "*.dll", "*.exe" };
            var files = new List<string>();
            foreach (var pattern in patterns) {
                files.AddRange(basePaths.SelectMany(s => Directory.GetFiles(s, pattern, allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
            }

            return files.ToArray();
        }
    }
}
