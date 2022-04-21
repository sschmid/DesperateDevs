﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins
{
    public class TargetFrameworkProfilePreProcessor : IPreProcessor, IConfigurable
    {
        public string Name => "Fix Target Framework Profile";
        public int Order => 0;
        public bool RunInDryMode => true;

        public Dictionary<string, string> DefaultProperties => _projectPathConfig.DefaultProperties;

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
        }

        public void PreProcess()
        {
            var project = File.ReadAllText(_projectPathConfig.ProjectPath);
            project = removeTargetFrameworkProfile(project);
            File.WriteAllText(_projectPathConfig.ProjectPath, project);
        }

        string removeTargetFrameworkProfile(string project)
        {
            const string pattern1 = @"\s*<TargetFrameworkProfile>Unity Subset v3.5</TargetFrameworkProfile>";
            const string pattern2 = @"\s*<TargetFrameworkProfile>Unity Full v3.5</TargetFrameworkProfile>";
            project = Regex.Replace(project, pattern1, string.Empty);
            project = Regex.Replace(project, pattern2, string.Empty);
            return project;
        }
    }
}
