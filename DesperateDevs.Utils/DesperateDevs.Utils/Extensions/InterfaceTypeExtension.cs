using System;

namespace DesperateDevs.Utils {

    public static class InterfaceTypeExtension {

        public static bool ImplementsInterface<T>(this Type type) {
            return !type.IsInterface && type.GetInterface(typeof(T).FullName) != null;
        }
    }
}
