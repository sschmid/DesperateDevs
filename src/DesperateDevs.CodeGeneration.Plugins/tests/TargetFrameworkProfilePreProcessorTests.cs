using DesperateDevs.Tests;
using Xunit;

namespace DesperateDevs.CodeGeneration.Plugins.Tests
{
    public class TargetFrameworkProfilePreProcessorTests
    {
        [Fact(Skip = "Manual Test")]
        public void UpdatesCsproj()
        {
            var preProcessor = new TargetFrameworkProfilePreProcessor();
            var preferences = new TestPreferences(
                "DesperateDevs.CodeGeneration.Plugins.ProjectPath = Tests/Tests/DesperateDevs.CodeGeneration.Plugins/TestTargetFrameworkProfilePreProcessor.csproj"
            );
            preProcessor.Configure(preferences);
            preProcessor.PreProcess();
        }
    }
}
