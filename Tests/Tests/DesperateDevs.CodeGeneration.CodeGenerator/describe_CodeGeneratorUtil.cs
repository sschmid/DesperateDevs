using System.IO;
using DesperateDevs.CodeGeneration.CodeGenerator;
using NSpec;
using Shouldly;

class describe_CodeGeneratorUtil : nspec {

    void when_autoImport() {

        var projectRoot = TestExtensions.GetProjectRoot();
        const string searchPaths = "CodeGeneratorUtil";

        CodeGeneratorConfig config = null;

        before = () => {
            config = new CodeGeneratorConfig();
            var preferences = new TestPreferences(string.Empty);
            preferences.properties.AddProperties(config.defaultProperties, true);
            config.Configure(preferences);
        };

        xit["updates searchPaths in CodeGeneratorConfig"] = () => {
            CodeGeneratorUtil.AutoImport(config, projectRoot + Path.DirectorySeparatorChar +
                                                 searchPaths + Path.DirectorySeparatorChar
                                                 + "/TestPlugins");

            config.searchPaths.Length.ShouldBe(2);
            config.searchPaths[0].ShouldBe(searchPaths + "/TestPlugins/One");
            config.searchPaths[1].ShouldBe(searchPaths + "/TestPlugins/Two");
        };

        it["updates searchPaths when path contains spaces"] = () => {
            CodeGeneratorUtil.AutoImport(config, projectRoot + Path.DirectorySeparatorChar +
                                                 searchPaths + Path.DirectorySeparatorChar +
                                                 "/Test Plugins");

            config.searchPaths.Length.ShouldBe(2);
            config.searchPaths[0].ShouldBe(searchPaths + "/Test Plugins/One");
            config.searchPaths[1].ShouldBe(searchPaths + "/Test Plugins/Two");
        };
    }
}
