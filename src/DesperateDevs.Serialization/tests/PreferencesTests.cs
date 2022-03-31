using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Serialization.Tests.Fixtures;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Serialization.Tests
{
    public class PreferencesTests
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization", "tests", "Fixtures");

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
        public void CanToMinifiedString()
        {
            Preferences.ToMinifiedString().Should().Be("key=value\n");
        }

        [Fact]
        public void HasUserKey()
        {
            UserPreferences.HasKey("userName").Should().BeTrue();
            var keys = UserPreferences.Keys.ToArray();
            keys.Length.Should().Be(2);
            keys.Should().Contain("key");
            keys.Should().Contain("userName");
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

        [Fact]
        public void LoadsPreferencesFromDisk()
        {
            var properties = Path.Combine(FixturesPath, "TestPreferences.properties");
            File.Exists(properties).Should().BeTrue();
            var preferences = new Preferences(properties, null);
            preferences["key"].Should().Be("value");
        }

        [Fact]
        public void LoadsUserPreferencesFromDisk()
        {
            var properties = Path.Combine(FixturesPath, "TestPreferences.properties");
            var userProperties = Path.Combine(FixturesPath, "TestUserPreferences.properties");
            File.Exists(properties).Should().BeTrue();
            File.Exists(userProperties).Should().BeTrue();
            var preferences = new Preferences(properties, userProperties);
            preferences["key"].Should().Be("Max");
        }

        [Fact]
        public void ReloadsUserPreferences()
        {
            var properties = Path.Combine(FixturesPath, "TestPreferences.properties");
            var userProperties = Path.Combine(FixturesPath, "TestUserPreferences.properties");
            var preferences = new Preferences(properties, userProperties);
            preferences["key"] = "test";
            preferences.Reload();
            preferences["key"].Should().Be("Max");
        }

        [Fact]
        public void LoadsDoubleQuotedPreferencesFromDisk()
        {
            var properties = Path.Combine(FixturesPath, "DoubleQuotedTestPreferences.properties");
            File.Exists(properties).Should().BeTrue();
            var preferences = new Preferences(properties, null, true);
            preferences["key"].Should().Be("value");
        }

        [Fact]
        public void SavesPreferencesToDisk()
        {
            var properties = Path.Combine(FixturesPath, "TestPreferences.properties");
            var userProperties = Path.Combine(FixturesPath, "TestUserPreferences.properties");

            var temp = Path.Combine(FixturesPath, "Temp");
            var tempProperties = Path.Combine(temp, "TestPreferences.properties");
            var tempUserProperties = Path.Combine(temp, "TestUserPreferences.properties");

            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            File.Copy(properties, tempProperties, true);
            File.Copy(userProperties, tempUserProperties, true);

            var preferences = new Preferences(tempProperties, tempUserProperties);
            preferences["key"] = "test";
            preferences.Save();

            File.ReadAllText(tempProperties).Should().Be("key = test\n");
            File.ReadAllText(tempUserProperties).Should().Be(File.ReadAllText(userProperties));
        }

        [Fact]
        public void SavesMinifiedPreferencesToDisk()
        {
            var properties = Path.Combine(FixturesPath, "TestPreferences.properties");
            var userProperties = Path.Combine(FixturesPath, "TestUserPreferences.properties");

            var temp = Path.Combine(FixturesPath, "Temp");
            var tempProperties = Path.Combine(temp, "TestPreferences.properties");
            var tempUserProperties = Path.Combine(temp, "TestUserPreferences.properties");

            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            File.Copy(properties, tempProperties, true);
            File.Copy(userProperties, tempUserProperties, true);

            var preferences = new Preferences(tempProperties, tempUserProperties);
            preferences["key"] = "test";
            preferences.Save(true);

            File.ReadAllText(tempProperties).Should().Be("key=test\n");
            File.ReadAllText(tempUserProperties).Should().Be(File.ReadAllText(userProperties).Replace(" ", string.Empty));
        }
    }
}
