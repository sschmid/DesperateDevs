using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace DesperateDevs.Tests.Cli
{
    static class Program
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string SolutionPath = Path.Combine(ProjectRoot, "..", "DesperateDevs.sln");

        public static async Task Main(string[] args)
        {
            await PrintProjectDependencies(args.Single());
        }

        static async Task PrintProjectDependencies(string project)
        {
            var sln = await OpenSolutionAsync();
            Console.WriteLine(project);
            Console.WriteLine(string.Join(
                "\n",
                GetDependencies(sln.Projects.Single(p => p.Name == project)))
            );
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

        static IEnumerable<string> GetDependencies(Project project)
        {
            // TODO Use roslyn to analyze project references and find usages
            // Cannot parse sdk-style csproj, so manually analyse with xml for now

            var xml = new XmlDocument();
            xml.Load(project.FilePath);
            return xml.SelectNodes("Project/ItemGroup/ProjectReference")
                .Cast<XmlNode>()
                .Select(reference => reference.Attributes[0].Value)
                .Select(path => Path.GetFileNameWithoutExtension(path));
        }
    }
}
