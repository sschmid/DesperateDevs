using System;
using System.IO;
using System.Linq;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Serialization.Cli.Utils.Tests
{
    [Collection("DesperateDevs.Serialization.Cli.Utils.Tests")]
    public class RemoveKeyValueCommandTests : IDisposable
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization.Cli.Utils", "tests", "fixtures");
        static readonly string TempPath = Path.Combine(FixturesPath, "temp", nameof(RemoveKeyValueCommandTests));

        readonly RemoveKeyValueCommand _command;

        Preferences Preferences => new Preferences(AbstractPreferencesCommand.DefaultPropertiesPath, null);

        public RemoveKeyValueCommandTests()
        {
            _command = new RemoveKeyValueCommand();
        }

        [Fact]
        public void ThrowsWhenNotCorrectNumberOfArgs()
        {
            WriteTestPreferences("key = value");
            FluentActions.Invoking(() => Run()).Should().Throw<Exception>();
        }

        [Fact]
        public void RemovesKey()
        {
            WriteTestPreferences("key = value");
            Run("-y", "key");
            Preferences.HasKey("key").Should().BeFalse();
        }

        [Fact]
        public void DoesNotRemoveKeyThatDoesNotExist()
        {
            WriteTestPreferences("key = value");
            Run("-y", "unknown");
        }

        [Fact]
        public void RemovesValueFromKey()
        {
            WriteTestPreferences("key = value1, value2, value3");
            Run("-y", "key", "value2");
            Preferences["key"].Should().Be("value1, value3");
        }

        [Fact]
        public void DoesNotRemoveValueThatDoesNotExist()
        {
            WriteTestPreferences("key = value1, value2, value3");
            Run("-y", "key", "unknown");
            Preferences["key"].Should().Be("value1, value2, value3");
        }

        [Fact]
        public void DoesNotRemoveValueFromKeyThatDoesNotExist()
        {
            WriteTestPreferences("key = value1, value2, value3");
            Run("-y", "unknown", "value2");
            Preferences["key"].Should().Be("value1, value2, value3");
        }

        void Run(params string[] args)
        {
            var list = args.ToList();
            list.Insert(0, _command.Trigger);
            _command.Run(null, list.ToArray());
        }

        void WriteTestPreferences(string properties)
        {
            Directory.CreateDirectory(TempPath);
            AbstractPreferencesCommand.DefaultPropertiesPath = Path.Combine(TempPath, "TestPreferences.properties");
            File.WriteAllText(AbstractPreferencesCommand.DefaultPropertiesPath, properties);
        }

        public void Dispose() => Directory.Delete(TempPath, true);
    }
}
