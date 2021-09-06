using FluentAssertions;
using Xunit;

namespace DesperateDevs.CodeGeneration.Tests
{
    public class CodeGenFileTests
    {
        [Fact]
        public void SetFields()
        {
            var file = new CodeGenFile("name.cs", "content", "MyGenerator");
            file.fileName.Should().Be("name.cs");
            file.fileContent.Should().Be("content");
            file.generatorName.Should().Be("MyGenerator");
        }

        [Fact]
        public void ConvertsToUnixNewLines()
        {
            var file = new CodeGenFile(null, "line1\r\nline2\r", null);
            file.fileContent.Should().Be("line1\nline2\n");
        }

        [Fact]
        public void ConvertsToUnixNewLinesWhenSettingFileContent()
        {
            var file = new CodeGenFile(null, string.Empty, null);
            file.fileContent = "line1\r\nline2\r";
            file.fileContent.Should().Be("line1\nline2\n");
        }
    }
}
