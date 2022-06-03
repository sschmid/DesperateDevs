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
    public class AddKeyValueCommandTests : IDisposable
    {
        static readonly LogMessageFormatter Formatter = new LogMessageFormatter("[{1}]\t{0}: {2}");

        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization.Cli.Utils", "tests", "fixtures");
        static readonly string TempPath = Path.Combine(FixturesPath, "temp");

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
            WriteTestPreferences();
        }

        [Fact]
        public void FailsWhenNotCorrectNumberOfArgs()
        {
            Run();
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(1);

            _logs.Clear();
            Run("testKey");
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(1);
        }

        [Fact]
        public void AddsValueToNewKey()
        {
            Run("-y", "testKey", "test value");
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(0);
            Preferences["testKey"].Should().Be("test value");
        }

        [Fact]
        public void AddsValueToExistingKey()
        {
            Run("-y", "key", "test value");
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(0);
            Preferences["key"].Should().Be("value, test value");
        }

        [Fact]
        public void DoesNotAddValueToNewKeyWhenNo()
        {
            Run("-n", "testKey", "test value");
            _logs.Count(log => log.LogLevel == LogLevel.Error).Should().Be(0);
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
            if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
            AbstractPreferencesCommand.DefaultPropertiesPath = Path.Combine(TempPath, "TestPreferences.properties");
            File.WriteAllText(AbstractPreferencesCommand.DefaultPropertiesPath, "key = value");
        }

        public void Dispose()
        {
            _output.WriteLine("Dispose");
            Logger.ResetAppenders();
            Logger.ResetLoggers();
        }
    }
}
