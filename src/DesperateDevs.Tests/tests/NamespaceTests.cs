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
        static readonly Dictionary<string, string> SourceFiles = TestHelper.ReadSourceFiles(ProjectRoot);

        readonly ITestOutputHelper _output;

        public NamespaceTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void RoughlyProcessesTheCorrectNumberOfFiles()
        {
            SourceFiles.Count.Should().BeGreaterThan(140);
            SourceFiles.Count.Should().BeLessThan(160);
        }

        [Theory, MemberData(nameof(Namespaces))]
        public void FileHasCorrectNamespace(FileNamespace ns)
        {
            var shortPath = ns.Path.RemoveProjectRoot(ProjectRoot);
            var isTest = shortPath.Contains(Path.DirectorySeparatorChar + "tests" + Path.DirectorySeparatorChar) &&
                         !shortPath.Contains("DesperateDevs.Tests");

            if (isTest)
                ns.Expected += ".Tests";

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
                    var fileName = kvp.Key.RemoveProjectRoot(ProjectRoot);
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
    }

    public struct FileNamespace
    {
        public string Path;
        public string Expected;
        public string Found;
    }
}
