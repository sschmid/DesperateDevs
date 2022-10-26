using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DesperateDevs.Tests
{
    public class NamespaceTests
    {
        readonly ITestOutputHelper _testOutputHelper;
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly Dictionary<string, string> SourceFiles = ReadSourceFiles(ProjectRoot);

        public NamespaceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void RoughlyProcessesTheCorrectNumberOfFiles()
        {
            foreach (var file in SourceFiles.OrderBy(kvp => kvp.Key))
                _testOutputHelper.WriteLine(file.Key.Replace(ProjectRoot + Path.DirectorySeparatorChar, string.Empty));

            SourceFiles.Count.Should().BeGreaterThan(90);
            SourceFiles.Count.Should().BeLessThan(110);
        }

        [Theory, MemberData(nameof(Namespaces))]
        public void FileHasCorrectNamespace(FileNamespace ns)
        {
            var shortPath = ns.Path.Replace(ProjectRoot + Path.DirectorySeparatorChar, string.Empty);
            _testOutputHelper.WriteLine(shortPath);

            var isTest = shortPath.StartsWith("tests");
            var isFixture = shortPath.Contains($"{Path.DirectorySeparatorChar}fixtures{Path.DirectorySeparatorChar}");
            var isBenchmark = shortPath.StartsWith("benchmarks");

            if (isTest)
                ns.Expected += ".Tests";

            if (isFixture)
                ns.Expected += ".Fixtures";

            if (isBenchmark)
                ns.Expected += ".Benchmarks";

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
            .Where(p => new[] {"obj", "bin", Path.Combine("DesperateDevs", "unity")}
                .All(ignore => !p.Contains(Path.DirectorySeparatorChar + ignore + Path.DirectorySeparatorChar)))
            .ToDictionary(p => p, File.ReadAllText);

        static string RemoveProjectRoot(string path, string projectRoot) => path
            .Replace(Path.Combine(projectRoot, "src") + Path.DirectorySeparatorChar, string.Empty)
            .Replace(Path.Combine(projectRoot, "tests") + Path.DirectorySeparatorChar, string.Empty)
            .Replace(Path.Combine(projectRoot, "fixtures") + Path.DirectorySeparatorChar, string.Empty)
            .Replace(Path.Combine(projectRoot, "benchmarks") + Path.DirectorySeparatorChar, string.Empty)
            .Replace(Path.Combine(projectRoot, "samples") + Path.DirectorySeparatorChar, string.Empty);
    }

    public struct FileNamespace
    {
        public string Path;
        public string Expected;
        public string Found;

        public override string ToString() => $"{Expected} - {Found}.{System.IO.Path.GetFileNameWithoutExtension(Path)}";
    }
}
