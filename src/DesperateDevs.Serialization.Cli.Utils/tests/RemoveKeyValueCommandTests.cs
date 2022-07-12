using System;
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
    [Collection("Non-Parallel")]
    public class RemoveKeyValueCommandTests : IDisposable
    {
        static readonly LogMessageFormatter Formatter = new LogMessageFormatter("[{1}]\t{0}: {2}");

        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization.Cli.Utils", "tests", "fixtures");
        static readonly string TempPath = Path.Combine(FixturesPath, "temp");

        readonly ITestOutputHelper _output;
        readonly List<(LogLevel LogLevel, string Message)> _logs;
        readonly RemoveKeyValueCommand _command;

        Preferences Preferences => new Preferences(AbstractPreferencesCommand.DefaultPropertiesPath, null);

        public RemoveKeyValueCommandTests(ITestOutputHelper output)
        {
            _output = output;
            _logs = new List<(LogLevel, string)>();
            Logger.AddAppender((logger, level, message) => _output.WriteLine(Formatter.FormatMessage(logger, level, message)));
            Logger.AddAppender((_, level, message) => _logs.Add((level, message)));
            _command = new RemoveKeyValueCommand();
        }

        [Fact]
        public void FailsWhenNotCorrectNumberOfArgs()
        {
            Run();
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(1);
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
            _logs.Count(log => log.LogLevel == LogLevel.Warn).Should().Be(1);
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
            _logs.Count(log => log.LogLevel == LogLevel.Warn).Should().Be(1);
            Preferences["key"].Should().Be("value1, value2, value3");
        }

        [Fact]
        public void DoesNotRemoveValueFromKeyThatDoesNotExist()
        {
            WriteTestPreferences("key = value1, value2, value3");
            Run("-y", "unknown", "value2");
            _logs.Count(log => log.LogLevel == LogLevel.Warn).Should().Be(1);
            Preferences["key"].Should().Be("value1, value2, value3");
        }

        void Run(params string[] args)
        {
            var list = args.ToList();
            list.Insert(0, _command.Trigger);
            _command.Run(null, list.ToArray());
        }

        void WriteTestPreferences(string value)
        {
            if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
            AbstractPreferencesCommand.DefaultPropertiesPath = Path.Combine(TempPath, "TestPreferences.properties");
            File.WriteAllText(AbstractPreferencesCommand.DefaultPropertiesPath, value);
        }

        public void Dispose()
        {
            _output.WriteLine("Dispose");
            Logger.GlobalLogLevel = LogLevel.On;
            Logger.ResetAppenders();
            Logger.ResetLoggers();
        }
    }
}
