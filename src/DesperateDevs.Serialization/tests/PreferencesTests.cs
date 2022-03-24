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
        TestPreferences DoubleQuotedPreferences => _preferences ?? (_preferences = new TestPreferences("key = \"value\"", null, true));
        TestPreferences _preferences;

        [Fact]
        public void SetsProperties()
        {
            Preferences.Properties.Count.Should().Be(1);
            Preferences.Properties.HasKey("key").Should().BeTrue();
            Preferences.UserProperties.Count.Should().Be(0);
        }

        [Fact]
        public void GetsValueForKey()
        {
            Preferences["key"].Should().Be("value");
        }

        [Fact]
        public void ThrowsWhenGettingMissingKey()
        {
            FluentActions.Invoking(() => Preferences["unknown"])
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
            UserPreferences.Properties.Values.Should().NotContain("Max");
        }

        [Fact]
        public void OverwritesValueWhenDifferent()
        {
            UserPreferences["key"] = "Jack";
            UserPreferences.Properties.Values.Should().Contain("Jack");
            UserPreferences.UserProperties.Values.Should().NotContain("Jack");
        }

        [Fact]
        public void UserPropertiesOverwriteDefaultProperties()
        {
            new TestPreferences(
                "key = ${userName}",
                "key = Overwrite"
            )["key"].Should().Be("Overwrite");
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

        [Fact]
        public void ReadsValuesFromDoubleQuotedValues()
        {
            DoubleQuotedPreferences["key"].Should().Be("value");
            DoubleQuotedPreferences.ToString().Should().Be("key = \"value\"\n");
        }

        [Fact]
        public void ResetsDoubleQuotedProperties()
        {
            DoubleQuotedPreferences.Reset(true);
            DoubleQuotedPreferences["key2"] = "value2";
            DoubleQuotedPreferences["key2"].Should().Be("value2");
            DoubleQuotedPreferences.ToString().Should().Be("key2 = \"value2\"\n");
        }
    }
}
