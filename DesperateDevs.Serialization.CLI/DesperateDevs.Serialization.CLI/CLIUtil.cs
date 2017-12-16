﻿using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.CLI;
using DesperateDevs.Utils;

namespace DesperateDevs.Serialization.CLI {

    public static class CLIUtil {

        public static Dictionary<string, string> GetDefaultProperties() {
            return new Dictionary<string, string>().Merge(
                AppDomain.CurrentDomain
                    .GetInstancesOf<ICommand>()
                    .OfType<IConfigurable>()
                    .Select(instance => instance.defaultProperties)
                    .ToArray());
        }
    }
}
