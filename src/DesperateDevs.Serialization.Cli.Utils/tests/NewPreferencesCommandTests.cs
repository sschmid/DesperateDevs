using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Serialization.Cli.Utils.Tests
{
    [Collection("DesperateDevs.Serialization.Cli.Utils.Tests")]
    public class NewPreferencesCommandTests : IDisposable
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization.Cli.Utils", "tests", "fixtures");
        static readonly string TempPath = Path.Combine(FixturesPath, "temp", nameof(NewPreferencesCommandTests));

        readonly NewPreferencesCommand _command;

        public NewPreferencesCommandTests()
        {
            Directory.CreateDirectory(TempPath);
            _command = new NewPreferencesCommand();
        }

        [Fact]
        public void ThrowsWhenInvalidProperty()
        {
            FluentActions.Invoking(() => Run()).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatesNewPreferencesWithDefaultValues()
        {
            var propertiesPath = Path.Combine(TempPath, "New.properties");
            Run("-s", propertiesPath);
            File.Exists(propertiesPath).Should().BeTrue();
            File.ReadAllText(propertiesPath).Should().Be("TestKey1 = TestValue1\nTestKey2 = TestValue2\n");
        }

        [Fact]
        public void CreatesNewPreferencesWithUserProperties()
        {
            var propertiesPath = Path.Combine(TempPath, "New.properties");
            var userPropertiesPath = Path.Combine(TempPath, "New.userproperties");
            Run("-s", propertiesPath, userPropertiesPath);
            File.Exists(propertiesPath).Should().BeTrue();
            File.Exists(userPropertiesPath).Should().BeTrue();
        }

        [Fact]
        public void DoesNotCreateNewPreferencesIfPropertiesExists()
        {
            var propertiesPath = Path.Combine(TempPath, "New.properties");
            File.WriteAllText(propertiesPath, "test = value");
            Run("-s", propertiesPath);
            File.ReadAllText(propertiesPath).Should().Be("test = value");
        }

        [Fact]
        public void DoesNotCreateNewPreferencesIfUserPropertiesExists()
        {
            var propertiesPath = Path.Combine(TempPath, "New.properties");
            var userPropertiesPath = Path.Combine(TempPath, "New.userproperties");
            File.WriteAllText(userPropertiesPath, "test = value");

            Run("-s", propertiesPath, userPropertiesPath);
            File.ReadAllText(userPropertiesPath).Should().Be("test = value");
            File.Exists(propertiesPath).Should().BeFalse();
        }

        [Fact]
        public void ForceOverwritesExistingPreferences()
        {
            var propertiesPath = Path.Combine(TempPath, "New.properties");
            var userPropertiesPath = Path.Combine(TempPath, "New.userproperties");
            File.WriteAllText(propertiesPath, "test = value");
            File.WriteAllText(userPropertiesPath, "test = value");

            Run("-s", "-f", propertiesPath, userPropertiesPath);
            File.ReadAllText(propertiesPath).Should().Be("TestKey1 = TestValue1\nTestKey2 = TestValue2\n");
            File.ReadAllText(userPropertiesPath).Should().Be(string.Empty);
        }

        void Run(params string[] args)
        {
            var list = args.ToList();
            list.Insert(0, _command.Trigger);
            _command.Run(null, list.ToArray());
        }

        public void Dispose() => Directory.Delete(TempPath, true);
    }

    class TestConfig : IConfigurable
    {
        public Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {"TestKey1", "TestValue1"},
            {"TestKey2", "TestValue2"}
        };

        public void Configure(Preferences preferences) { }
    }
}
