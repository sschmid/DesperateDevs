﻿using System;
using System.Linq;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public static class ArgsExtension
    {
        public static bool IsForce(this string[] args) => args.Any(arg => arg == "-f");

        public static string GetPropertiesPath(this string[] args) =>
            args.SingleOrDefault(arg => arg.EndsWith(".properties", StringComparison.OrdinalIgnoreCase));

        public static string GetUserPropertiesPath(this string[] args) =>
            args.SingleOrDefault(arg => arg.EndsWith(".userproperties", StringComparison.OrdinalIgnoreCase));
    }
}
