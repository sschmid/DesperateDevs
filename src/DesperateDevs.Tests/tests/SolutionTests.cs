using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Xunit;
using Xunit.Abstractions;

namespace DesperateDevs.Tests
{
    public class SolutionTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string SolutionPath = Path.Combine(ProjectRoot, "..", "DesperateDevs.sln");

        readonly ITestOutputHelper _output;

        public SolutionTests(ITestOutputHelper output) => _output = output;

        public static IEnumerable<object[]> Projects => OpenSolutionAsync().Result
            .Projects
            .Where(project => !project.HasDocuments)
            .Select(project => { return new object[] {project.Name, project}; });

        [Fact]
        public void HasUnixProjectPaths()
        {
            File.ReadAllText(SolutionPath).Should().NotContain("\\");
        }

        [Theory, MemberData(nameof(Projects))]
        public void ProjectsHaveUnixProjectReferencePaths(string projectName, Project project)
        {
            File.ReadAllText(project.FilePath).Should().NotContain("\\");
        }

        [Theory, MemberData(nameof(Projects))]
        public void OnlyHasUsedProjectReferences(string projectName, Project project)
        {
            // TODO Use roslyn to analyze project references and find usages
            // Cannot parse sdk-style csproj, so manually analyse with xml for now

            var xml = new XmlDocument();
            xml.Load(project.FilePath);
            var references = xml.SelectNodes("Project/ItemGroup/ProjectReference")
                .Cast<XmlNode>()
                .Select(reference => reference.Attributes[0].Value);

            foreach (var reference in references)
            {
                var projectNamespace = Path.GetFileNameWithoutExtension(reference);
                var files = Directory.EnumerateFiles(Path.GetDirectoryName(project.FilePath), "*.cs", SearchOption.TopDirectoryOnly).ToArray();
                var filesWithUsage = files
                    .Select(file => File.ReadAllText(file))
                    .Where(code => code.Contains(projectNamespace))
                    .ToArray();

                filesWithUsage.Length.Should().BeGreaterThan(0);
            }
        }

        static Task<Solution> OpenSolutionAsync()
        {
            if (!MSBuildLocator.IsRegistered) MSBuildLocator.RegisterDefaults();
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.LoadMetadataForReferencedProjects = true;
                return workspace.OpenSolutionAsync(SolutionPath);
            }
        }
    }
}
