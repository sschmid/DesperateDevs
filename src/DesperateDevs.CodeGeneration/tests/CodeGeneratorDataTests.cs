using FluentAssertions;
using Xunit;

namespace DesperateDevs.CodeGeneration.Tests
{
    public class CodeGeneratorDataTests
    {
        readonly CodeGeneratorData _data;

        public CodeGeneratorDataTests()
        {
            _data = new CodeGeneratorData {["TestKey"] = "TestValue"};
        }

        [Fact]
        public void AddsKeyAndValue()
        {
            _data["TestKey"].Should().Be("TestValue");
        }

        [Fact]
        public void ReplacesPlaceholders()
        {
            _data.ReplacePlaceholders("Hello, ${TestKey}")
                .Should().Be("Hello, TestValue");
        }

        [Fact]
        public void RespectsCaseOfPlaceholder()
        {
            _data.ReplacePlaceholders("Hello, ${testKey}")
                .Should().Be("Hello, testValue");
        }

        [Fact]
        public void ClonesData()
        {
            var clone = new CodeGeneratorData(_data);
            clone.Count.Should().Be(_data.Count);
            clone["TestKey"].Should().Be("TestValue");
        }
    }
}
