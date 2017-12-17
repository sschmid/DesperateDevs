using System.IO;
using System.Linq;
using System.Reflection;

namespace DesperateDevs.Utils {

    public partial class AssemblyResolver {

        public static Assembly[] GetAssembliesContainingType<T>(params string[] basePaths) {
            var resolver = new AssemblyResolver(true, basePaths);
            foreach (var file in basePaths.SelectMany(Directory.GetFiles)) {
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

        public static AssemblyResolver LoadAssembliesContainingType<T>(params string[] basePaths) {
            var assemblies = GetAssembliesContainingType<T>(basePaths);

            var resolver = new AssemblyResolver(false);
            foreach (var assembly in assemblies) {
                resolver.Load(assembly.CodeBase);
            }

            return resolver;
        }
    }
}
