using System.IO;
using DesperateDevs.Serialization.Tests.Fixtures;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace Jenny.Plugins.Tests
{
    public class TargetFrameworkProfilePreProcessorTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "Jenny.Plugins", "tests", "fixtures");

        [Fact]
        public void RemovesUnitySubsetv35()
        {
            var temp = Path.Combine(FixturesPath, "temp");
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            var project = Path.Combine(FixturesPath, "UnitySubsetv35.csproj");
            var tempProject = Path.Combine(temp, "UnitySubsetv35.csproj");

            File.Copy(project, tempProject, true);

            var preProcessor = new TargetFrameworkProfilePreProcessor();
            var preferences = new TestPreferences($"Jenny.Plugins.ProjectPath = {tempProject}");
            preProcessor.Configure(preferences);
            preProcessor.PreProcess();

            File.ReadAllText(tempProject).Should().NotContain("Unity Subset v3.5");
        }

        [Fact]
        public void RemovesUnityFullv35()
        {
            var temp = Path.Combine(FixturesPath, "temp");
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            var project = Path.Combine(FixturesPath, "UnityFullv35.csproj");
            var tempProject = Path.Combine(temp, "UnityFullv35.csproj");

            File.Copy(project, tempProject, true);

            var preProcessor = new TargetFrameworkProfilePreProcessor();
            var preferences = new TestPreferences($"Jenny.Plugins.ProjectPath = {tempProject}");
            preProcessor.Configure(preferences);
            preProcessor.PreProcess();

            File.ReadAllText(tempProject).Should().NotContain("Unity Full v3.5");
        }
    }
}
