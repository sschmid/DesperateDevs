using System.IO;
using DesperateDevs.Serialization.Tests.Fixtures;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace Jenny.Generator.Tests
{
    public class CodeGeneratorUtilTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();

        static readonly string SearchPaths = Path.Combine(
            "src", "Jenny.Generator", "tests", "Fixtures"
        );

        readonly CodeGeneratorConfig _config;

        public CodeGeneratorUtilTests()
        {
            _config = new CodeGeneratorConfig();
            var preferences = new TestPreferences(string.Empty);
            preferences.Properties.AddProperties(_config.DefaultProperties, true);
            _config.Configure(preferences);
        }

        [Fact(Skip = "Add project to build test plugin dlls")]
        public void UpdatesSearchPathsInCodeGeneratorConfig()
        {
            CodeGeneratorUtil.AutoImport(_config, Path.Combine(ProjectRoot, SearchPaths, "/TestPlugins"));
            _config.SearchPaths.Length.Should().Be(2);
            _config.SearchPaths[0].Should().Be(SearchPaths + "/TestPlugins/One");
            _config.SearchPaths[1].Should().Be(SearchPaths + "/TestPlugins/Two");
        }

        [Fact(Skip = "Add project to build test plugin dlls")]
        public void UpdatesSearchPathsWhenPathContainsSpaces()
        {
            CodeGeneratorUtil.AutoImport(_config, Path.Combine(ProjectRoot, SearchPaths, "/Test Plugins"));
            _config.SearchPaths.Length.Should().Be(2);
            _config.SearchPaths[0].Should().Be(SearchPaths + "/Test Plugins/One");
            _config.SearchPaths[1].Should().Be(SearchPaths + "/Test Plugins/Two");
        }
    }
}
