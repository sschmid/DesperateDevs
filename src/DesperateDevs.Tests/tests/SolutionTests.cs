using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
            .Where(p => !p.HasDocuments)
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

        static Task<Solution> OpenSolutionAsync()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.LoadMetadataForReferencedProjects = true;
                return workspace.OpenSolutionAsync(SolutionPath);
            }
        }
    }
}
