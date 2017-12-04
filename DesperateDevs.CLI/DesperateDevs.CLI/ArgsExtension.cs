using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.CLI {

    public static class ArgsExtension {

        static readonly HashSet<string> defaultParameter = new HashSet<string> {
            "-v",
            "-s",
            "-d"
        };

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
                if (defaultParameter.Contains(arg)) {
                    argsList.Remove(arg);
                }
            }

            return argsList.ToArray();
        }
    }
}
