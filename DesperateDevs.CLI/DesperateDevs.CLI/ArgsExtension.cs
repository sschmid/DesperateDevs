using System.Linq;

namespace DesperateDevs.CLI {

    public static class ArgsExtension {

        public static bool isForce(this string[] args) {
            return args.Any(arg => arg == "-f");
        }

        public static bool isVerbose(this string[] args) {
            return args.Any(arg => arg == "-v") || isDebug(args);
        }

        public static bool isSilent(this string[] args) {
            return args.Any(arg => arg == "-s");
        }

        public static bool isDebug(this string[] args) {
            return args.Any(arg => arg == "-d");
        }

        public static string[] WithoutTrigger(this string[] args) {
            var argsList = args.ToList();
            argsList.RemoveAt(0);
            return argsList.ToArray();
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
