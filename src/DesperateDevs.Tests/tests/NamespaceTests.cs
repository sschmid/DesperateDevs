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
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly Dictionary<string, string> SourceFiles = ReadSourceFiles(ProjectRoot);

        readonly ITestOutputHelper _output;

        public NamespaceTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void RoughlyProcessesTheCorrectNumberOfFiles()
        {
            SourceFiles.Count.Should().BeGreaterThan(160);
            SourceFiles.Count.Should().BeLessThan(180);
        }

        [Theory, MemberData(nameof(Namespaces))]
        public void FileHasCorrectNamespace(FileNamespace ns)
        {
            var shortPath = RemoveProjectRoot(ns.Path, ProjectRoot);
            var isTest = shortPath.Contains(Path.DirectorySeparatorChar + "tests" + Path.DirectorySeparatorChar) &&
                         !shortPath.Contains("DesperateDevs.Tests");

            var isFixture = shortPath.Contains(Path.DirectorySeparatorChar + "Fixtures" + Path.DirectorySeparatorChar);

            if (isTest)
                ns.Expected += ".Tests";

            if (isFixture)
                ns.Expected += ".Fixtures";

            _output.WriteLine($"{ns.Expected} is the namespace of {shortPath}");
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
            .Where(p => new[] {"obj", "fixtures", Path.Combine("tests", "bin")}
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
    }
}
