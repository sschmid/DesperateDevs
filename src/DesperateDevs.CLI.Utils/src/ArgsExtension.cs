using System;
using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.Cli.Utils {

    public static class ArgsExtension {

        static readonly HashSet<string> _defaultParameter = new HashSet<string> {
            "-v",
            "-s",
            "-d"
        };

        public static bool IsVerbose(this string[] args) {
            return HasParameter(args, "-v") || IsDebug(args);
        }

        public static bool IsSilent(this string[] args) {
            return HasParameter(args, "-s");
        }

        public static bool IsDebug(this string[] args) {
            return HasParameter(args, "-d");
        }

        public static bool HasParameter(this string[] args, string parameter) {
            return args.Any(arg => arg == parameter);
        }

        // TODO rename to WithoutCommand
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
