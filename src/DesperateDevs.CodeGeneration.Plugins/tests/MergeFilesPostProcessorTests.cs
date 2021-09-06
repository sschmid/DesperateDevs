using FluentAssertions;
using Xunit;

namespace DesperateDevs.CodeGeneration.Plugins.Tests
{
    public class MergeFilesPostProcessorTests
    {
        [Fact]
        public void MergesFilesWithSameFilename()
        {
            var files = new[]
            {
                new CodeGenFile("file1", "content1", "gen1"),
                new CodeGenFile("file1", "content2", "gen2"),
                new CodeGenFile("file3", "content3", "gen3")
            };

            var postprocessor = new MergeFilesPostProcessor();
            files = postprocessor.PostProcess(files);

            files.Length.Should().Be(2);
            files[0].fileName.Should().Be("file1");
            files[1].fileName.Should().Be("file3");

            files[0].fileContent.Should().Be("content1\ncontent2");
            files[0].generatorName.Should().Be("gen1, gen2");
        }
    }
}
