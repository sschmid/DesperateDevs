using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DesperateDevs.Extensions
{
    public static class AppDomainExtension
    {
        public static IEnumerable<Type> GetAllTypes(this AppDomain appDomain) =>
            GetAllTypes(appDomain.GetAssemblies());

        public static IEnumerable<Type> GetAllTypes(this IEnumerable<Assembly> assemblies)
        {
            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types.AddRange(ex.Types.Where(type => type != null));
                }
            }

            return types;
        }

        public static IEnumerable<Type> GetNonAbstractTypes<T>(this AppDomain appDomain) =>
            GetNonAbstractTypes<T>(GetAllTypes(appDomain));

        public static IEnumerable<Type> GetNonAbstractTypes<T>(this IEnumerable<Type> types) => types
            .Where(type => !type.IsAbstract)
            .Where(type => type.ImplementsInterface<T>());

        public static IEnumerable<T> GetInstancesOf<T>(this AppDomain appDomain) =>
            GetInstancesOf<T>(GetNonAbstractTypes<T>(appDomain));

        public static IEnumerable<T> GetInstancesOf<T>(this IEnumerable<Type> types) =>
            GetNonAbstractTypes<T>(types).Select(type => (T)Activator.CreateInstance(type));
    }
}
