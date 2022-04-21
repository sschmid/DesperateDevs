﻿using System.Collections.Generic;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins
{
    public class ProjectPathConfig : AbstractConfigurableConfig
    {
        readonly string _projectPathKey = $"{nameof(DesperateDevs.CodeGeneration.Plugins)}.ProjectPath";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {_projectPathKey, "Assembly-CSharp.csproj"}
        };

        public string ProjectPath => _preferences[_projectPathKey];
    }
}