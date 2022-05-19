using DesperateDevs.Serialization.Tests.Fixtures;
using Xunit;

namespace Jenny.Plugins.Tests
{
    public class TargetFrameworkProfilePreProcessorTests
    {
        [Fact(Skip = "Manual Test")]
        public void UpdatesCsproj()
        {
            var preProcessor = new TargetFrameworkProfilePreProcessor();
            var preferences = new TestPreferences(
                "Jenny.Plugins.ProjectPath = Tests/Tests/Jenny.Plugins/TestTargetFrameworkProfilePreProcessor.csproj"
            );
            preProcessor.Configure(preferences);
            preProcessor.PreProcess();
        }
    }
}
