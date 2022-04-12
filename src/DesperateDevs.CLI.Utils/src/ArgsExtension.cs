using System;
using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.Cli.Utils
{
    public static class ArgsExtension
    {
        static readonly HashSet<string> DefaultParameter = new HashSet<string>
        {
            "-v",
            "-s",
            "-d"
        };

        public static bool IsVerbose(this string[] args) => HasParameter(args, "-v") || IsDebug(args);
        public static bool IsSilent(this string[] args) => HasParameter(args, "-s");
        public static bool IsDebug(this string[] args) => HasParameter(args, "-d");
        public static bool HasParameter(this string[] args, string parameter) => args.Any(arg => arg == parameter);
        public static string[] WithoutTrigger(this string[] args) => args.Skip(1).ToArray();

        public static string[] WithoutDefaultParameter(this string[] args) => args
            .Where(arg => !DefaultParameter.Contains(arg))
            .ToArray();

        public static string[] WithoutParameter(this string[] args) => args
            .Where(arg => !arg.StartsWith("-", StringComparison.Ordinal))
            .ToArray();
    }
}
