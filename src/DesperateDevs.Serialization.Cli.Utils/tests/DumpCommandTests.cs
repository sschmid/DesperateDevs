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
    [Collection("DesperateDevs.Serialization.Cli.Utils.Tests")]
    public class DumpCommandTests : IDisposable
    {
        static readonly LogMessageFormatter Formatter = new LogMessageFormatter("{2}");

        static readonly string ProjectRoot = TestHelper.GetProjectRoot();
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "DesperateDevs.Serialization.Cli.Utils", "tests", "fixtures");
        static readonly string TempPath = Path.Combine(FixturesPath, "temp", nameof(DumpCommandTests));

        readonly ITestOutputHelper _output;
        readonly List<(LogLevel LogLevel, string Message)> _logs;
        readonly DumpCommand _command;

        public DumpCommandTests(ITestOutputHelper output)
        {
            _output = output;
            _logs = new List<(LogLevel, string)>();
            Logger.AddAppender((logger, level, message) => _output.WriteLine(Formatter.FormatMessage(logger, level, message)));
            Logger.AddAppender((_, level, message) =>
            {
                if (level != LogLevel.Debug) _logs.Add((level, message));
            });
            _command = new DumpCommand();
        }

        [Fact]
        public void DumpsEmptyPreferences()
        {
            WriteTestPreferences(string.Empty);
            Run();
            _logs[0].Message.Should().Be(string.Empty);
        }

        [Fact]
        public void DumpsValue()
        {
            WriteTestPreferences("key = value");
            Run();
            _logs[0].Message.Should().Be(@"key: value
");
        }

        [Fact]
        public void DumpsMultipleValues()
        {
            WriteTestPreferences("key1 = value1\nkey2 = value2");
            Run();
            _logs[0].Message.Should().Be(@"key1: value1
key2: value2
");
        }

        [Fact]
        public void DumpsMultiValueKey()
        {
            WriteTestPreferences("key = value1, value2, value3");
            Run();
            _logs[0].Message.Should().Be(@"key:
- value1
- value2
- value3
");
        }

        [Fact]
        public void DumpsMultipleMultiValueKeys()
        {
            WriteTestPreferences("key1 = value1, value2, value3\nkey2 = value4, value5, value6");
            Run();
            _logs[0].Message.Should().Be(@"key1:
- value1
- value2
- value3
key2:
- value4
- value5
- value6
");
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

        public void Dispose()
        {
            Directory.Delete(TempPath, true);
            _output.WriteLine("Dispose");
            Logger.GlobalLogLevel = LogLevel.On;
            Logger.ResetAppenders();
            Logger.ResetLoggers();
        }
    }
}
