using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins {

    public class TargetFrameworkProfilePreProcessor : IPreProcessor, IConfigurable {

        public string name { get { return "Fix Target Framework Profile"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _projectPathConfig.defaultProperties; } }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();

        public void Configure(Preferences preferences) {
            _projectPathConfig.Configure(preferences);
        }

        public void PreProcess() {
            var project = File.ReadAllText(_projectPathConfig.projectPath);
            project = removeTargetFrameworkProfile(project);
            File.WriteAllText(_projectPathConfig.projectPath, project);
        }

        string removeTargetFrameworkProfile(string project) {
            const string pattern = @"\s*<TargetFrameworkProfile>Unity Subset v3.5</TargetFrameworkProfile>";
            project = Regex.Replace(project, pattern, string.Empty);
            return project;
        }
    }
}
