using System;
using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.CLI {

    public static class ArgsExtension {

        static readonly HashSet<string> _defaultParameter = new HashSet<string> {
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
            return args.Skip(1).ToArray();
        }

        public static string[] WithoutDefaultParameter(this string[] args) {
            return args
                .Where(arg => !_defaultParameter.Contains(arg))
                .ToArray();
        }

        public static string[] WithoutParameter(this string[] args) {
            return args
                .Where(arg => !arg.StartsWith("-", StringComparison.Ordinal))
                .ToArray();
        }
    }
}
