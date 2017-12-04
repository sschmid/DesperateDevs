using System;
using System.Linq;

namespace DesperateDevs.Utils {

    public static class InterfaceTypeExtension {

        public static bool ImplementsInterface<T>(this Type type) {
            if (!type.IsInterface && type.GetInterfaces().Contains(typeof(T))) {
                return true;
            }

            return false;
        }
    }
}
