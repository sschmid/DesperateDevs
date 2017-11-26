using System.Linq;

namespace DesperateDevs.CLI {

    public static class ArgsExtension {

        public static bool isForce(this string[] args) {
            return args.Any(arg => arg == "-f");
        }

        public static bool isVerbose(this string[] args) {
            return args.Any(arg => arg == "-v");
        }

        public static bool isSilent(this string[] args) {
            return args.Any(arg => arg == "-s");
        }

        public static string[] WithoutParameter(this string[] args) {
            var argsList = args.ToList();

            foreach (var arg in args) {
                if (arg.StartsWith("-")) {
                    argsList.Remove(arg);
                }
            }

            return argsList.ToArray();
        }
    }
}
