using FluentAssertions;
using Xunit;

namespace Sherlog.Tests
{
    public class SherlogTests
    {
        public SherlogTests()
        {
            Logger.ResetLoggers();
            Logger.ResetAppenders();
            Logger.GlobalLogLevel = LogLevel.On;
        }

        [Fact]
        public void CreatesNewLogger()
        {
            var logger = Logger.GetLogger("TestLogger");
            logger.Should().NotBeNull();
            logger.GetType().Should().BeSameAs(typeof(Logger));
            logger.Name.Should().Be("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.On);
        }

        [Fact]
        public void CreatesNewLoggerForType()
        {
            var logger = Logger.GetLogger(typeof(SherlogTests));
            logger.Should().NotBeNull();
            logger.GetType().Should().BeSameAs(typeof(Logger));
            logger.Name.Should().Be("Sherlog.Tests.SherlogTests");
            logger.LogLevel.Should().Be(LogLevel.On);
        }

        [Fact]
        public void ReturnsSameLoggerWhenNameIsEqual()
        {
            var logger1 = Logger.GetLogger("TestLogger");
            var logger2 = Logger.GetLogger("TestLogger");
            logger1.Should().BeSameAs(logger2);
        }

        [Fact]
        public void ReturnsSameLoggerWhenTypeIsEqual()
        {
            var logger1 = Logger.GetLogger(typeof(SherlogTests));
            var logger2 = Logger.GetLogger(typeof(SherlogTests));
            logger1.Should().BeSameAs(logger2);
        }

        [Fact]
        public void ClearsCreatedLoggers()
        {
            var logger1 = Logger.GetLogger("TestLogger");
            Logger.ResetLoggers();
            var logger2 = Logger.GetLogger("TestLogger");
            logger1.Should().NotBeSameAs(logger2);
        }

        [Fact]
        public void CreatesNewLoggerWithGlobalLogLevel()
        {
            Logger.GlobalLogLevel = LogLevel.Error;
            var logger = Logger.GetLogger("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void SetsGlobalLogLevelOnCreatedLogger()
        {
            var logger = Logger.GetLogger("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.On);
            Logger.GlobalLogLevel = LogLevel.Error;
            logger.LogLevel.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void CreatesNewLoggerWithGlobalAppender()
        {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            Logger.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });

            var appenderLogLevel2 = LogLevel.Off;
            var appenderMessage2 = string.Empty;
            Logger.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel2 = logLevel;
                appenderMessage2 = message;
            });

            var logger = Logger.GetLogger("TestLogger");
            logger.Info("test message");

            appenderLogLevel.Should().Be(LogLevel.Info);
            appenderMessage.Should().Be("test message");
            appenderLogLevel2.Should().Be(LogLevel.Info);
            appenderMessage2.Should().Be("test message");
        }

        [Fact]
        public void AddsAppenderOnCreatedLogger()
        {
            var logger = Logger.GetLogger("TestLogger");
            var didLog = false;
            Logger.AddAppender((log, logLevel, message) => didLog = true);
            logger.Info("test message");
            didLog.Should().BeTrue();
        }

        [Fact]
        public void RemovesAppenderOnCreatedLogger()
        {
            var didLog = false;
            LogDelegate appender = (log, logLevel, message) => didLog = true;
            Logger.AddAppender(appender);
            var logger = Logger.GetLogger("TestLogger");
            Logger.RemoveAppender(appender);
            logger.Info("test message");
            didLog.Should().BeFalse();
        }

        [Fact]
        public void ClearsGlobalAppenders()
        {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            Logger.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });
            Logger.ResetAppenders();
            var logger = Logger.GetLogger("TestLogger");
            logger.Info("test message");
            appenderLogLevel.Should().Be(LogLevel.Off);
            appenderMessage.Should().Be(string.Empty);
        }
    }
}
