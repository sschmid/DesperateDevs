using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Xunit;

namespace DesperateDevs.Tests
{
    public class SolutionTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string SolutionPath = Path.Combine(ProjectRoot, "..", "DesperateDevs.sln");

        public static IEnumerable<object[]> Projects => OpenSolutionAsync().Result
            .Projects
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
            foreach (var reference in project.AllProjectReferences)
            {
                var projectNamespace = Path.GetFileNameWithoutExtension(reference.ProjectId.ToString());
                var files = Directory.EnumerateFiles(Path.GetDirectoryName(project.FilePath), "*.cs", SearchOption.TopDirectoryOnly);
                var filesWithUsage = files
                    .Select(file => File.ReadAllText(file))
                    .Where(code => code.Contains(projectNamespace));

                filesWithUsage.Count().Should().BeGreaterThan(0, projectNamespace);
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
