using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DesperateDevs.Serialization;

namespace DesperateDevs.CodeGeneration.Plugins
{
    public class TargetFrameworkProfilePreProcessor : IPreProcessor, IConfigurable
    {
        public string Name
        {
            get { return "Fix Target Framework Profile"; }
        }

        public int Order
        {
            get { return 0; }
        }

        public bool RunInDryMode
        {
            get { return true; }
        }

        public Dictionary<string, string> DefaultProperties
        {
            get { return _projectPathConfig.DefaultProperties; }
        }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
        }

        public void PreProcess()
        {
            var project = File.ReadAllText(_projectPathConfig.projectPath);
            project = removeTargetFrameworkProfile(project);
            File.WriteAllText(_projectPathConfig.projectPath, project);
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
