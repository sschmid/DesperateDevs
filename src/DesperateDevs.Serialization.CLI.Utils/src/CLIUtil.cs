using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;

namespace DesperateDevs.Serialization.Cli.Utils
{
    public static class CLIUtil
    {
        public static Dictionary<string, string> GetDefaultProperties()
        {
            return new Dictionary<string, string>().Merge(
                AppDomain.CurrentDomain
                    .GetInstancesOf<IConfigurable>()
                    .Select(instance => instance.DefaultProperties)
                    .ToArray());
        }
    }
}
