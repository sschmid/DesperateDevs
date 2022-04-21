using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Extensions;

namespace DesperateDevs.CodeGeneration.Plugins
{
    public class UpdateCSProjPostProcessor : IPostProcessor, IConfigurable
    {
        public string Name
        {
            get { return "Update .csproj"; }
        }

        public int Order
        {
            get { return 96; }
        }

        public bool RunInDryMode
        {
            get { return false; }
        }

        public Dictionary<string, string> DefaultProperties
        {
            get
            {
                return _projectPathConfig.DefaultProperties
                    .Merge(_targetDirectoryConfig.DefaultProperties);
            }
        }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
            _targetDirectoryConfig.Configure(preferences);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files)
        {
            var project = File.ReadAllText(_projectPathConfig.projectPath);
            project = removeExistingGeneratedEntries(project);
            project = addGeneratedEntries(project, files);
            File.WriteAllText(_projectPathConfig.projectPath, project);
            return files;
        }

        string removeExistingGeneratedEntries(string project)
        {
            var escapedTargetDirectory = _targetDirectoryConfig.targetDirectory
                .Replace("/", "\\")
                .Replace("\\", "\\\\");

            var entryPattern = @"\s*<Compile Include=""" + escapedTargetDirectory + @".* \/>";
            project = Regex.Replace(project, entryPattern, string.Empty);

            const string emptyItemGroup = @"\s*<ItemGroup>\s*<\/ItemGroup>";
            project = Regex.Replace(project, emptyItemGroup, string.Empty);

            return project;
        }

        string addGeneratedEntries(string project, CodeGenFile[] files)
        {
            const string endOfItemGroupPattern = @"<\/ItemGroup>";

            const string generatedEntriesTemplate =
                @"</ItemGroup>
  <ItemGroup>
{0}
  </ItemGroup>";

            var entryTemplate = @"    <Compile Include=""" + _targetDirectoryConfig.targetDirectory.Replace("/", "\\") + @"\{0}"" />";

            var entries = string.Join("\r\n", files.Select(
                file => string.Format(entryTemplate, file.FileName.Replace("/", "\\"))).ToArray());

            var generatedEntries = string.Format(generatedEntriesTemplate, entries);

            return new Regex(endOfItemGroupPattern).Replace(project, generatedEntries, 1);
        }
    }
}
