using System.IO;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Tests
{
    public class CodeGeneratorUtilTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();

        static readonly string SearchPaths = Path.Combine(
            "src", "DesperateDevs.CodeGeneration.CodeGenerator", "tests", "Fixtures"
        );

        readonly CodeGeneratorConfig _config;

        public CodeGeneratorUtilTests()
        {
            _config = new CodeGeneratorConfig();
            var preferences = new TestPreferences(string.Empty);
            preferences.properties.AddProperties(_config.defaultProperties, true);
            _config.Configure(preferences);
        }

        [Fact(Skip = "Add project to build test plugin dlls")]
        public void UpdatesSearchPathsInCodeGeneratorConfig()
        {
            CodeGeneratorUtil.AutoImport(_config, Path.Combine(ProjectRoot, SearchPaths, "/TestPlugins"));
            _config.searchPaths.Length.Should().Be(2);
            _config.searchPaths[0].Should().Be(SearchPaths + "/TestPlugins/One");
            _config.searchPaths[1].Should().Be(SearchPaths + "/TestPlugins/Two");
        }

        [Fact(Skip = "Add project to build test plugin dlls")]
        public void UpdatesSearchPathsWhenPathContainsSpaces()
        {
            CodeGeneratorUtil.AutoImport(_config, Path.Combine(ProjectRoot, SearchPaths, "/Test Plugins"));
            _config.searchPaths.Length.Should().Be(2);
            _config.searchPaths[0].Should().Be(SearchPaths + "/Test Plugins/One");
            _config.searchPaths[1].Should().Be(SearchPaths + "/Test Plugins/Two");
        }
    }
}
