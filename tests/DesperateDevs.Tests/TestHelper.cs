using System.IO;

namespace DesperateDevs.Tests
{
    public static class TestHelper
    {
        public static string GetProjectRoot()
        {
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (!File.Exists(Path.Combine(current.FullName, "DesperateDevs.sln"))) current = current.Parent;
            return current.FullName;
        }
    }
}
