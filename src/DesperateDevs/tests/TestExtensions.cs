using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DesperateDevs.Tests
{
    public static class TestExtensions
    {
        const string ProjectName = "DesperateDevs";

        public static void Wait(int ms = 50) => Thread.Sleep(ms);

        public static string GetProjectRoot()
        {
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current.Name != ProjectName) current = current.Parent;
            return current.FullName;
        }

        public static Dictionary<string, string> GetSourceFiles(string path)
        {
            return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(p => !new[]
                {
                    "Tests", "TestFixtures", "Fixtures",
                    "TestDependency", "TestDependencyBase", "TestUpdateCSProjPostProcessor",
                    "Libraries", "Generated", "Examples",
                    "Readme", "Build",
                    "bin", "obj", "packages", "AssemblyInfo.cs",
                    "Commands.cs", "Program.cs"
                }.Any(p.Contains))
                .ToDictionary(p => p, File.ReadAllText);
        }

        public static Dictionary<string, string> GetSourceFilesInclAllProjects(string path)
        {
            return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(p => !new[]
                {
                    "Libraries", "Generated",
                    "Build", "bin", "obj", "AssemblyInfo.cs"
                }.Any(p.Contains))
                .ToDictionary(p => p, File.ReadAllText);
        }
    }
}
