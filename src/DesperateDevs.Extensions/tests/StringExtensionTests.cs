using FluentAssertions;
using Xunit;

namespace DesperateDevs.Extensions.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void UppercaseFirst() => "test".UppercaseFirst().Should().Be("Test");

        [Fact]
        public void UppercaseFirstHandlesEmptyString() => string.Empty.UppercaseFirst().Should().Be(string.Empty);

        [Fact]
        public void LowercaseFirst() => "Test".LowercaseFirst().Should().Be("test");

        [Fact]
        public void LowercaseFirstHandlesEmptyString() => string.Empty.LowercaseFirst().Should().Be(string.Empty);

        [Fact]
        public void ToUnixLineEndings() => "1\r\n2\r3\n".ToUnixLineEndings().Should().Be("1\n2\n3\n");

        [Fact]
        public void ToUnixPath() => "a/b\\c\\d/e".ToUnixPath().Should().Be("a/b/c/d/e");

        [Fact]
        public void ToCsv() => new[] {"1", " 2", "", "3 "}.ToCSV(false, false).Should().Be("1, 2, , 3");

        [Fact]
        public void ToCsvMinified() => new[] {"1", " 2", "", "3 "}.ToCSV(true, false).Should().Be("1,2,,3");

        [Fact]
        public void ToCsvRemoveEmptyEntries() => new[] {"1", " 2", "", "3 "}.ToCSV(false, true).Should().Be("1, 2, 3");

        [Fact]
        public void ToCsvMinifiedRemoveEmptyEntries() => new[] {"1", " 2", "", "3 "}.ToCSV(true, true).Should().Be("1,2,3");

        [Fact]
        public void FromCsv() => "1,2 ,, 3".FromCSV(false).Should().BeEquivalentTo("1", "2", "", "3");

        [Fact]
        public void FromCsvRemoveEmptyEntries() => "1,2 ,, 3".FromCSV(true).Should().BeEquivalentTo("1", "2", "3");

        [Fact]
        public void ToSpacedCamelCase() => "ThisIsATest".ToSpacedCamelCase().Should().Be("This Is A Test");

        [Fact]
        public void MakesPathRelativeToParentDirs() => "/home/DesperateDevs/test/file".MakePathRelativeTo("/home/DesperateDevs").Should().Be("test/file");

        [Fact]
        public void CannotMakePathRelativeToSubDirs() => "/home".MakePathRelativeTo("/home/DesperateDevs").Should().Be("/home");
    }
}
