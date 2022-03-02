using System.Collections.Generic;
using DesperateDevs.Serialization.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Serialization.Tests
{
    public class PreferencesTests
    {
        TestPreferences Preferences => _preferences ?? (_preferences = new TestPreferences("key = value"));
        TestPreferences UserPreferences => _preferences ?? (_preferences = new TestPreferences("key = ${userName}", "userName = Max"));
        TestPreferences _preferences;

        [Fact]
        public void SetsProperties()
        {
            Preferences.properties.count.Should().Be(1);
            Preferences.properties.HasKey("key").Should().BeTrue();
            Preferences.userProperties.count.Should().Be(0);
        }

        [Fact]
        public void GetsValueForKey()
        {
            Preferences["key"].Should().Be("value");
        }

        [Fact]
        public void ThrowsWhenGettingMissingKey()
        {
            Preferences.Invoking(p => p["unknown"])
                .Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void HasKey()
        {
            Preferences.HasKey("key").Should().BeTrue();
        }

        [Fact]
        public void SetsKey()
        {
            Preferences["key2"] = "value2";
            Preferences["key2"].Should().Be("value2");
            Preferences.HasKey("key2").Should().BeTrue();
        }

        [Fact]
        public void CanToString()
        {
            Preferences.ToString().Should().Be("key = value\n");
        }

        [Fact(Skip = "TODO")]
        public void SupportsDoubleQuoteMode()
        {
            Preferences.doubleQuoteMode = true;
            Preferences["key2"] = "value2";
            Preferences["key2"].Should().Be("\"value2\"");
        }

        [Fact]
        public void HasUserKey()
        {
            UserPreferences.HasKey("userName").Should().BeTrue();
        }

        [Fact]
        public void ResolvesPlaceholderFromUserProperties()
        {
            UserPreferences["key"].Should().Be("Max");
        }

        [Fact]
        public void DoesNotOverwriteValueWhenNotDifferent()
        {
            UserPreferences["key"] = "Max";
            UserPreferences.properties.ToString().Contains("Max").Should().BeFalse();
        }

        [Fact]
        public void OverwritesValueWhenDifferent()
        {
            UserPreferences["key"] = "Jack";
            UserPreferences.properties.ToString().Contains("Jack").Should().BeTrue();
        }

        [Fact]
        public void UserPropertiesOverwriteDefaultProperties()
        {
            var p = new TestPreferences(
                "key = ${userName}",
                "key = Overwrite");
            p["key"].Should().Be("Overwrite");
        }

        [Fact]
        public void ResetsDefaultProperties()
        {
            UserPreferences["newKey"] = "newValue";
            UserPreferences.Reset();
            UserPreferences.HasKey("newKey").Should().BeFalse();
            UserPreferences.HasKey("userName").Should().BeTrue();
        }

        [Fact]
        public void ResetsAllProperties()
        {
            UserPreferences["newKey"] = "newValue";
            UserPreferences.Reset(true);
            UserPreferences.HasKey("newKey").Should().BeFalse();
            UserPreferences.HasKey("userName").Should().BeFalse();
        }

        [Fact]
        public void CanToStringWithUserProperties()
        {
            UserPreferences.ToString().Should().Be("key = ${userName}\nuserName = Max\n");
        }
    }
}
