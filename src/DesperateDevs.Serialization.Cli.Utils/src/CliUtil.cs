using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public static class CliUtil
    {
        public static Dictionary<string, string> GetDefaultProperties() =>
            new Dictionary<string, string>().Merge(
                AppDomain.CurrentDomain
                    .GetInstancesOf<IConfigurable>()
                    .Select(instance => instance.DefaultProperties));
    }
}
