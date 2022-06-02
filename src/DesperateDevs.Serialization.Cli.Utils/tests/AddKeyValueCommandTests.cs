using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.Tests;
using FluentAssertions;
using Sherlog;
using Sherlog.Formatters;
using Xunit;
using Xunit.Abstractions;

namespace DesperateDevs.Serialization.Cli.Utils.Tests
{
    public class AddKeyValueCommandTests
    {
        static readonly LogMessageFormatter Formatter = new LogMessageFormatter("[{1}]\t{0}: {2}");

        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization.Cli.Utils", "tests", "fixtures");

        readonly ITestOutputHelper _output;
        readonly List<(LogLevel LogLevel, string Message)> _logs;
        readonly AddKeyValueCommand _command;

        Preferences Preferences => new Preferences(AbstractPreferencesCommand.DefaultPropertiesPath, null);

        public AddKeyValueCommandTests(ITestOutputHelper output)
        {
            _output = output;
            _logs = new List<(LogLevel, string)>();
            Logger.AddAppender((logger, level, message) => _output.WriteLine(Formatter.FormatMessage(logger, level, message)));
            Logger.AddAppender((_, level, message) => _logs.Add((level, message)));
            _command = new AddKeyValueCommand();
            SetupPreferences();
        }

        [Fact]
        public void FailsWhenNotCorrectNumberOfArgs()
        {
            _command.Run(null, new[]
            {
                _command.Trigger
            });
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(1);

            _logs.Clear();
            _command.Run(null, new[]
            {
                _command.Trigger,
                "testKey"
            });
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(1);
        }

        [Fact]
        public void AddsValueToNewKey()
        {
            _command.Run(null, new[]
            {
                _command.Trigger,
                "-y",
                "testKey",
                "test value"
            });

            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(0);
            Preferences["testKey"].Should().Be("test value");
        }

        [Fact]
        public void AddsValueToExistingKey()
        {
            _command.Run(null, new[]
            {
                _command.Trigger,
                "-y",
                "key",
                "test value"
            });

            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(0);
            Preferences["key"].Should().Be("value, test value");
        }

        [Fact]
        public void DoesNotAddValueToNewKeyWhenNo()
        {
            _command.Run(null, new[]
            {
                _command.Trigger,
                "-n",
                "testKey",
                "test value"
            });

            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(0);
            Preferences.HasKey("testKey").Should().BeFalse();
        }

        void SetupPreferences()
        {
            var temp = Path.Combine(FixturesPath, "temp");
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            var properties = Path.Combine(FixturesPath, "TestPreferences.properties");
            var tempProperties = Path.Combine(temp, "TestPreferences.properties");
            File.Copy(properties, tempProperties, true);
            AbstractPreferencesCommand.DefaultPropertiesPath = tempProperties;
        }
    }
}
