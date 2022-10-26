using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Tests
{
    public class NamespaceTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly Dictionary<string, string> SourceFiles = ReadSourceFiles(ProjectRoot);

        [Fact]
        public void RoughlyProcessesTheCorrectNumberOfFiles()
        {
            SourceFiles.Count.Should().BeGreaterThan(70);
            SourceFiles.Count.Should().BeLessThan(90);
        }

        [Theory, MemberData(nameof(Namespaces))]
        public void FileHasCorrectNamespace(FileNamespace ns)
        {
            var shortPath = RemoveProjectRoot(ns.Path, ProjectRoot);
            var isTest = shortPath.Contains($"{Path.DirectorySeparatorChar}tests{Path.DirectorySeparatorChar}");
            var isBenchmark = shortPath.Contains($"{Path.DirectorySeparatorChar}benchmarks{Path.DirectorySeparatorChar}");
            var isFixture = shortPath.Contains($"{Path.DirectorySeparatorChar}fixtures{Path.DirectorySeparatorChar}");

            if (isTest)
                ns.Expected += ".Tests";

            if (isBenchmark)
                ns.Expected += ".Benchmarks";

            if (isFixture)
                ns.Expected += ".Fixtures";

            ns.Found.Should().Be(ns.Expected, $"see {ns.Path}");
        }

        public static IEnumerable<object[]> Namespaces
        {
            get
            {
                var expectedNamespacePattern = $@"[^\{Path.DirectorySeparatorChar}]*";
                const string namespacePattern = @"(?:^namespace)\s.*\b";
                return SourceFiles.Select(kvp =>
                {
                    var fileName = RemoveProjectRoot(kvp.Key, ProjectRoot);
                    var expectedNamespace = Regex.Match(fileName, expectedNamespacePattern).ToString()
                        .Replace("namespace ", string.Empty).Trim();

                    var foundNamespace = Regex.Match(kvp.Value, namespacePattern, RegexOptions.Multiline).ToString()
                        .Replace("namespace ", string.Empty).Trim();

                    return new object[]
                    {
                        new FileNamespace
                        {
                            Path = kvp.Key,
                            Expected = expectedNamespace,
                            Found = foundNamespace
                        }
                    };
                });
            }
        }

        static Dictionary<string, string> ReadSourceFiles(string projectRoot) => Directory
            .GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories)
            .Where(p => RemoveProjectRoot(p, projectRoot).StartsWith("DesperateDevs"))
            .Where(p => new[] {"obj", "fixtures", Path.Combine("tests", "bin"), Path.Combine("DesperateDevs", "unity")}
                .All(ignore => !p.Contains(Path.DirectorySeparatorChar + ignore + Path.DirectorySeparatorChar)))
            .ToDictionary(p => p, File.ReadAllText);

        static string RemoveProjectRoot(string path, string projectRoot) =>
            path.Replace(projectRoot + Path.DirectorySeparatorChar, string.Empty);
    }

    public struct FileNamespace
    {
        public string Path;
        public string Expected;
        public string Found;

        public override string ToString() => $"{Expected} - {Found}.{System.IO.Path.GetFileNameWithoutExtension(Path)}";
    }
}
