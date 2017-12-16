using DesperateDevs.CodeGeneration.Plugins;
using NSpec;

class describe_TargetFrameworkProfilePreProcessor : nspec {

    void when_preprocessing() {

        xit["manual test"] = () => {
            var preProcessor = new TargetFrameworkProfilePreProcessor();
            var preferences = new TestPreferences(
                "DesperateDevs.CodeGeneration.Plugins.ProjectPath = Tests/Tests/DesperateDevs.CodeGeneration.Plugins/TestTargetFrameworkProfilePreProcessor.csproj");
            preProcessor.Configure(preferences);

            preProcessor.PreProcess();
        };
    }
}
