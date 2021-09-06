using System;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Logging.Tests
{
    public class LoggerTests
    {
        const string message = "hi";
        const string loggerName = "MyLogger";

        readonly Logger _logger;

        public LoggerTests()
        {
            _logger = new Logger("MyLogger");
        }

        [Theory]
        [InlineData(LogLevel.On, true, true, true, true, true, true)]
        [InlineData(LogLevel.Trace, true, true, true, true, true, true)]
        [InlineData(LogLevel.Debug, false, true, true, true, true, true)]
        [InlineData(LogLevel.Info, false, false, true, true, true, true)]
        [InlineData(LogLevel.Warn, false, false, false, true, true, true)]
        [InlineData(LogLevel.Error, false, false, false, false, true, true)]
        [InlineData(LogLevel.Fatal, false, false, false, false, false, true)]
        [InlineData(LogLevel.Off, false, false, false, false, false, false)]
        public void LogLevels(LogLevel logLevel, bool trace, bool debug, bool info, bool warn, bool error, bool fatal)
        {
            _logger.logLevel = logLevel;
            AssertLogLevel(_logger.Trace, LogLevel.Trace, trace);
            AssertLogLevel(_logger.Debug, LogLevel.Debug, debug);
            AssertLogLevel(_logger.Info, LogLevel.Info, info);
            AssertLogLevel(_logger.Warn, LogLevel.Warn, warn);
            AssertLogLevel(_logger.Error, LogLevel.Error, error);
            AssertLogLevel(_logger.Fatal, LogLevel.Fatal, fatal);

            void AssertLogLevel(Action<string> logMethod, LogLevel logLvl, bool shouldLog)
            {
                var didLog = false;
                var eventLogLevel = LogLevel.Off;
                string eventMessage = null;
                Logger eventLogger = null;
                _logger.OnLog += (logger, logLevel, msg) =>
                {
                    didLog = true;
                    eventLogger = logger;
                    eventLogLevel = logLevel;
                    eventMessage = msg;
                };

                logMethod(message);

                didLog.Should().Be(shouldLog);

                if (shouldLog)
                {
                    eventLogger.Should().BeSameAs(_logger);
                    eventMessage.Should().Be(message);
                    eventLogLevel.Should().Be(logLvl);
                }
                else
                {
                    eventMessage.Should().BeNull();
                    eventLogLevel.Should().Be(LogLevel.Off);
                    eventLogger.Should().BeNull();
                }
            }
        }

        [Fact]
        public void AssertDoesNotThrowWhenConditionIsTrue()
        {
            _logger.Assert(true, "success");
        }

        [Fact]
        public void AssertThrowsWhenConditionIsFalse()
        {
            _logger.Invoking(logger => logger.Assert(false, "fail"))
                .Should().Throw<FablAssertException>();
        }

        [Fact]
        public void ResetsOnLog()
        {
            var didLog = 0;
            _logger.OnLog += (logger, level, s) => didLog += 1;
            _logger.Reset();
            _logger.Info("Test");
            didLog.Should().Be(0);
        }
    }
}
