using System;
using System.Linq;

namespace DesperateDevs.Serialization.CLI {

    public static class ArgsExtension {

        public static bool isForce(this string[] args) {
            return args.Any(arg => arg == "-f");
        }

        public static string GetPropertiesPath(this string[] args) {
            return args.SingleOrDefault(arg => arg.EndsWith(".properties", StringComparison.OrdinalIgnoreCase));
        }

        public static string GetUserPropertiesPath(this string[] args) {
            return args.SingleOrDefault(arg => arg.EndsWith(".userproperties", StringComparison.OrdinalIgnoreCase))
                   ?? Environment.UserName + ".userproperties";
        }
    }
}
