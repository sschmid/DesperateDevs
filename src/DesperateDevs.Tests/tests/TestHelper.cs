using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DesperateDevs.Tests
{
    public static class TestHelper
    {
        public const string ProjectName = "DesperateDevs";

        public static void Wait(int ms = 50) => Thread.Sleep(ms);

        public static string GetProjectRoot()
        {
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current.Name != ProjectName) current = current.Parent;
            return Path.Combine(current.FullName, "src");
        }

        public static Dictionary<string, string> ReadSourceFiles(string projectRoot) => Directory
            .GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories)
            .Where(p => p.RemoveProjectRoot(projectRoot).StartsWith(ProjectName))
            .Where(p => new[] {"obj"}
                .All(ignore => !p.Contains(Path.DirectorySeparatorChar + ignore + Path.DirectorySeparatorChar)))
            .ToDictionary(p => p, File.ReadAllText);

        public static string RemoveProjectRoot(this string path, string projectRoot) =>
            path.Replace(projectRoot + Path.DirectorySeparatorChar, string.Empty);
    }
}
