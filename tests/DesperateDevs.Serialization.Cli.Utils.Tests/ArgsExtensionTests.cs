using FluentAssertions;
using Xunit;

namespace DesperateDevs.Serialization.Cli.Utils.Tests
{
    public class ArgsExtensionTests
    {
        [Fact]
        public void DoesNotGetPropertiesPath()
        {
            new[] {""}.GetPropertiesPath().Should().BeNull();
        }

        [Fact]
        public void GetsPropertiesPath()
        {
            const string properties = "test.properties";
            new[] {properties}.GetPropertiesPath().Should().Be(properties);
        }

        [Fact]
        public void DoesNotGetUserPropertiesPath()
        {
            new[] {""}.GetUserPropertiesPath().Should().BeNull();
        }

        [Fact]
        public void GetsUserPropertiesPath()
        {
            const string properties = "test.userproperties";
            new[] {properties}.GetUserPropertiesPath().Should().Be(properties);
        }
    }
}
