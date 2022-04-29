using DesperateDevs.Serialization.Tests.Fixtures;
using Xunit;

namespace DesperateDevs.CodeGeneration.Plugins.Tests
{
    public class UpdateCsProjPostProcessorTests
    {
        [Fact(Skip = "Manual Test")]
        public void UpdatesCsproj()
        {
            var postProcessor = new UpdateCsProjPostProcessor();
            var preferences = new TestPreferences(
                @"DesperateDevs.CodeGeneration.Plugins.ProjectPath = Tests/TestUpdateCSProjPostProcessor/TestUpdateCSProjPostProcessor.csproj
DesperateDevs.CodeGeneration.Plugins.TargetDirectory = Assets/Sources");
            postProcessor.Configure(preferences);

            var files = new[]
            {
                new CodeGenFile("My/Generated/Folder/File1.cs", "Hello, world!", "Test1"),
                new CodeGenFile("My/Generated/Folder/File2.cs", "Hello, world!", "Test2")
            };

            postProcessor.PostProcess(files);
        }
    }
}
