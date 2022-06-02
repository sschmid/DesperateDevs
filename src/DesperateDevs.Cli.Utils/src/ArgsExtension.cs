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
            "-d",
            "-y",
            "-n"
        };

        public static bool IsVerbose(this string[] args) => HasParameter(args, "-v") || IsDebug(args);
        public static bool IsSilent(this string[] args) => HasParameter(args, "-s");
        public static bool IsDebug(this string[] args) => HasParameter(args, "-d");

        public static bool IsYes(this string[] args) => HasParameter(args, "-y");
        public static bool IsNo(this string[] args) => HasParameter(args, "-n");

        public static bool HasParameter(this IEnumerable<string> args, string parameter) =>
            args.Any(arg => arg == parameter);

        public static IEnumerable<string> WithoutTrigger(this IEnumerable<string> args) =>
            args.Skip(1);

        public static IEnumerable<string> WithoutDefaultParameter(this IEnumerable<string> args) => args.Except(DefaultParameter);

        public static IEnumerable<string> WithoutParameter(this IEnumerable<string> args) =>
            args.Where(arg => !arg.StartsWith("-", StringComparison.Ordinal));
    }
}
