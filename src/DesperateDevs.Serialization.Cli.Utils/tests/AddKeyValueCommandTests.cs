using System;
using System.IO;
using System.Linq;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Serialization.Cli.Utils.Tests
{
    [Collection("DesperateDevs.Serialization.Cli.Utils.Tests")]
    public class AddKeyValueCommandTests : IDisposable
    {
        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization.Cli.Utils", "tests", "fixtures");
        static readonly string TempPath = Path.Combine(FixturesPath, "temp", nameof(AddKeyValueCommandTests));

        readonly AddKeyValueCommand _command;

        Preferences Preferences => new Preferences(AbstractPreferencesCommand.DefaultPropertiesPath, null);

        public AddKeyValueCommandTests()
        {
            WriteTestPreferences();
            _command = new AddKeyValueCommand();
        }

        [Fact]
        public void ThrowsWhenNotCorrectNumberOfArgs()
        {
            FluentActions.Invoking(() => Run()).Should().Throw<Exception>();
            FluentActions.Invoking(() => Run("testKey")).Should().Throw<Exception>();
        }

        [Fact]
        public void AddsValueToNewKey()
        {
            Run("-y", "testKey", "test value");
            Preferences["testKey"].Should().Be("test value");
        }

        [Fact]
        public void AddsValueToExistingKey()
        {
            Run("-y", "key", "test value");
            Preferences["key"].Should().Be("value, test value");
        }

        [Fact]
        public void DoesNotAddValueToNewKeyWhenNo()
        {
            Run("-n", "testKey", "test value");
            Preferences.HasKey("testKey").Should().BeFalse();
        }

        void Run(params string[] args)
        {
            var list = args.ToList();
            list.Insert(0, _command.Trigger);
            _command.Run(null, list.ToArray());
        }

        void WriteTestPreferences()
        {
            Directory.CreateDirectory(TempPath);
            AbstractPreferencesCommand.DefaultPropertiesPath = Path.Combine(TempPath, "TestPreferences.properties");
            File.WriteAllText(AbstractPreferencesCommand.DefaultPropertiesPath, "key = value");
        }

        public void Dispose() => Directory.Delete(TempPath, true);
    }
}
