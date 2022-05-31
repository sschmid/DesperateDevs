using System.IO;
using DesperateDevs.Extensions;
using DesperateDevs.Serialization.Tests.Fixtures;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace Jenny.Plugins.Tests
{
    public class UpdateCsProjPostProcessorTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "Jenny.Plugins", "tests", "fixtures");

        [Fact]
        public void AddsGeneratedFilesToUnity2020_3()
        {
            var project = UpdateCsproj("Unity-2020.3.csproj");
            project.Should().NotContain("Old");
            project.Should().Contain(@"  <ItemGroup>
    <Compile Include=""Assets/Sources/Generated/Test1/Test2/File1.cs"" />
    <Compile Include=""Assets/Sources/Generated/Test1/Test2/File2.cs"" />
  </ItemGroup>".ToUnixLineEndings());
        }

        [Fact]
        public void AddsGeneratedFilesToUnity2021_3()
        {
            var project = UpdateCsproj("Unity-2021.3.csproj");
            project.Should().NotContain("Old");
            project.Should().Contain(@"  <ItemGroup>
    <Compile Include=""Assets/Sources/Generated/Test1/Test2/File1.cs"" />
    <Compile Include=""Assets/Sources/Generated/Test1/Test2/File2.cs"" />
  </ItemGroup>".ToUnixLineEndings());
        }

        string UpdateCsproj(string csproj)
        {
            var temp = Path.Combine(FixturesPath, "temp");
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            var project = Path.Combine(FixturesPath, csproj);
            var tempProject = Path.Combine(temp, csproj);

            File.Copy(project, tempProject, true);

            var postProcessor = new UpdateCsProjPostProcessor();
            var preferences = new TestPreferences($@"
Jenny.Plugins.ProjectPath = {tempProject}
Jenny.Plugins.TargetDirectory = Assets/Sources");
            postProcessor.Configure(preferences);

            var files = new[]
            {
                new CodeGenFile("Test1/Test2/File1.cs", "test file content 1", "TestGenerator1"),
                new CodeGenFile("Test1/Test2/File2.cs", "test file content 2", "TestGenerator2")
            };

            postProcessor.PostProcess(files);
            return File.ReadAllText(tempProject).ToUnixLineEndings();
        }
    }
}
