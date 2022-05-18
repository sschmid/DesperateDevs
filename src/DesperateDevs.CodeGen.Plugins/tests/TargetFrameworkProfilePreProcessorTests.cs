using DesperateDevs.Serialization.Tests.Fixtures;
using Xunit;

namespace DesperateDevs.CodeGen.Plugins.Tests
{
    public class TargetFrameworkProfilePreProcessorTests
    {
        [Fact(Skip = "Manual Test")]
        public void UpdatesCsproj()
        {
            var preProcessor = new TargetFrameworkProfilePreProcessor();
            var preferences = new TestPreferences(
                "DesperateDevs.CodeGen.Plugins.ProjectPath = Tests/Tests/DesperateDevs.CodeGen.Plugins/TestTargetFrameworkProfilePreProcessor.csproj"
            );
            preProcessor.Configure(preferences);
            preProcessor.PreProcess();
        }
    }
}
