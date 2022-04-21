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
        public string Name => "Update .csproj";
        public int Order => 96;
        public bool RunInDryMode => false;

        public Dictionary<string, string> DefaultProperties =>
            _projectPathConfig.DefaultProperties.Merge(_targetDirectoryConfig.DefaultProperties);

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Preferences preferences)
        {
            _projectPathConfig.Configure(preferences);
            _targetDirectoryConfig.Configure(preferences);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files)
        {
            var project = File.ReadAllText(_projectPathConfig.ProjectPath);
            project = RemoveExistingGeneratedEntries(project);
            project = AddGeneratedEntries(project, files);
            File.WriteAllText(_projectPathConfig.ProjectPath, project);
            return files;
        }

        string RemoveExistingGeneratedEntries(string project)
        {
            var escapedTargetDirectory = _targetDirectoryConfig.TargetDirectory
                .Replace("/", "\\")
                .Replace("\\", "\\\\");

            var entryPattern = @"\s*<Compile Include=""" + escapedTargetDirectory + @".* \/>";
            project = Regex.Replace(project, entryPattern, string.Empty);

            const string emptyItemGroup = @"\s*<ItemGroup>\s*<\/ItemGroup>";
            project = Regex.Replace(project, emptyItemGroup, string.Empty);

            return project;
        }

        string AddGeneratedEntries(string project, CodeGenFile[] files)
        {
            const string endOfItemGroupPattern = @"<\/ItemGroup>";

            const string generatedEntriesTemplate =
                @"</ItemGroup>
  <ItemGroup>
{0}
  </ItemGroup>";

            var entryTemplate = @"    <Compile Include=""" + _targetDirectoryConfig.TargetDirectory.Replace("/", "\\") + @"\{0}"" />";

            var entries = string.Join("\r\n", files.Select(
                file => string.Format(entryTemplate, file.FileName.Replace("/", "\\"))).ToArray());

            var generatedEntries = string.Format(generatedEntriesTemplate, entries);

            return new Regex(endOfItemGroupPattern).Replace(project, generatedEntries, 1);
        }
    }
}
