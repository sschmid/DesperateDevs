using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.Plugins;
using NSpec;

class describe_UpdateCSProjPostProcessor : nspec {

    void when_post_processing() {

        xit["manual test"] = () => {
            var postProcessor = new UpdateCSProjPostProcessor();
            var preferences = new TestPreferences(
@"DesperateDevs.CodeGeneration.Plugins.ProjectPath = Tests/TestUpdateCSProjPostProcessor/TestUpdateCSProjPostProcessor.csproj
DesperateDevs.CodeGeneration.Plugins.TargetDirectory = Assets/Sources");
            postProcessor.Configure(preferences);

            var files = new [] {
                new CodeGenFile("My/Generated/Folder/File1.cs", "Hello, world!", "Test1"),
                new CodeGenFile("My/Generated/Folder/File2.cs", "Hello, world!", "Test2")
            };

            postProcessor.PostProcess(files);
        };
    }
}
