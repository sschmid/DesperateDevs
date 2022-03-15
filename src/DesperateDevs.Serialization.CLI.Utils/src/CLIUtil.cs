using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs;

namespace DesperateDevs.Serialization.CLI.Utils {

    public static class CLIUtil {

        public static Dictionary<string, string> GetDefaultProperties() {
            return new Dictionary<string, string>().Merge(
                AppDomain.CurrentDomain
                    .GetInstancesOf<IConfigurable>()
                    .Select(instance => instance.defaultProperties)
                    .ToArray());
        }
    }
}
