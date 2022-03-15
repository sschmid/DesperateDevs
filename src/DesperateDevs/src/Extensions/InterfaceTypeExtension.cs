using System;

namespace DesperateDevs {

    public static class InterfaceTypeExtension {

        public static bool ImplementsInterface<T>(this Type type) {
            return !type.IsInterface && type.GetInterface(typeof(T).FullName) != null;
        }
    }
}
