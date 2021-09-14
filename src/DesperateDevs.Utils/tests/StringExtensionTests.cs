using FluentAssertions;
using Xunit;

namespace DesperateDevs.Utils.Tests
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
        public void ToCsv() => new[] {"1", "2", "3"}.ToCSV().Should().Be("1, 2, 3");

        [Fact]
        public void ArrayFromCsv() => "1,2, 3".ArrayFromCSV().Should().BeEquivalentTo("1", "2", "3");

        [Fact]
        public void ToSpacedCamelCase() => "ThisIsATest".ToSpacedCamelCase().Should().Be("This Is A Test");
    }
}
