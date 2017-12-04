using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DesperateDevs.Utils {

    public static class AppDomainExtension {

        public static Type[] GetAllTypes(this AppDomain appDomain) {
            return GetAllTypes(appDomain.GetAssemblies());
        }

        public static Type[] GetAllTypes(this IEnumerable<Assembly> assemblies) {
            var types = new List<Type>();
            foreach (var assembly in assemblies) {
                try {
                    types.AddRange(assembly.GetTypes());
                } catch (ReflectionTypeLoadException ex) {
                    types.AddRange(ex.Types.Where(type => type != null));
                }
            }

            return types.ToArray();
        }

        public static Type[] GetNonAbstractTypes<T>(this AppDomain appDomain) {
            return GetNonAbstractTypes<T>(GetAllTypes(appDomain));
        }

        public static Type[] GetNonAbstractTypes<T>(this Type[] types) {
            return types
                .Where(type => !type.IsAbstract)
                .Where(type => type.ImplementsInterface<T>())
                .ToArray();
        }

        public static T[] GetInstancesOf<T>(this AppDomain appDomain) {
            return GetInstancesOf<T>(GetNonAbstractTypes<T>(appDomain));
        }

        public static T[] GetInstancesOf<T>(this Type[] types) {
            return GetNonAbstractTypes<T>(types)
                .Select(type => (T)Activator.CreateInstance(type))
                .ToArray();
        }
    }
}
