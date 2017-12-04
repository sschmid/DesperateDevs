using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.Plugins;
using DesperateDevs.Serialization;
using NSpec;

class describe_UpdateCSProjPostProcessor : nspec {

    readonly string pathPrefix = TestExtensions.GetProjectRoot() + "/Tests/TestFixtures/DesperateDevs.CodeGeneration.Plugins/";

    void when_post_processing() {

        xit["manual test"] = () => {
            var postProcessor = new UpdateCSProjPostProcessor();
            var preferences = new Preferences(pathPrefix + "Test.properties", pathPrefix + "EmptyUser.properties");
            postProcessor.Configure(preferences);

            var files = new [] {
                new CodeGenFile("My/Generated/Folder/File1.cs", "Hello, world!", "Test1"),
                new CodeGenFile("My/Generated/Folder/File2.cs", "Hello, world!", "Test2")
            };

            postProcessor.PostProcess(files);
        };
    }
}
