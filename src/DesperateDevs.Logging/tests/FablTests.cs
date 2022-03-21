using FluentAssertions;
using Xunit;

namespace DesperateDevs.Logging.Tests
{
    public class FablTests
    {
        public FablTests()
        {
            fabl.ResetLoggers();
            fabl.ResetAppenders();
            fabl.GlobalLogLevel = LogLevel.On;
        }

        [Fact]
        public void CreatesNewLogger()
        {
            var logger = fabl.GetLogger("TestLogger");
            logger.Should().NotBeNull();
            logger.GetType().Should().BeSameAs(typeof(Logger));
            logger.Name.Should().Be("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.On);
        }

        [Fact]
        public void ReturnsSameLoggerWhenNameIsEqual()
        {
            var logger1 = fabl.GetLogger("TestLogger");
            var logger2 = fabl.GetLogger("TestLogger");
            logger1.Should().BeSameAs(logger2);
        }

        [Fact]
        public void ClearsCreatedLoggers()
        {
            var logger1 = fabl.GetLogger("TestLogger");
            fabl.ResetLoggers();
            var logger2 = fabl.GetLogger("TestLogger");
            logger1.Should().NotBeSameAs(logger2);
        }

        [Fact]
        public void CreatesNewLoggerWithGlobalLogLevel()
        {
            fabl.GlobalLogLevel = LogLevel.Error;
            var logger = fabl.GetLogger("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void SetsGlobalLogLevelOnCreatedLogger()
        {
            var logger = fabl.GetLogger("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.On);
            fabl.GlobalLogLevel = LogLevel.Error;
            logger.LogLevel.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void CreatesNewLoggerWithGlobalAppender()
        {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            fabl.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });

            var appenderLogLevel2 = LogLevel.Off;
            var appenderMessage2 = string.Empty;
            fabl.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel2 = logLevel;
                appenderMessage2 = message;
            });

            var logger = fabl.GetLogger("TestLogger");
            logger.Info("test message");

            appenderLogLevel.Should().Be(LogLevel.Info);
            appenderMessage.Should().Be("test message");
            appenderLogLevel2.Should().Be(LogLevel.Info);
            appenderMessage2.Should().Be("test message");
        }

        [Fact]
        public void AddsAppenderOnCreatedLogger()
        {
            var logger = fabl.GetLogger("TestLogger");
            var didLog = false;
            fabl.AddAppender((log, logLevel, message) => didLog = true);
            logger.Info("test message");
            didLog.Should().BeTrue();
        }

        [Fact]
        public void RemovesAppenderOnCreatedLogger()
        {
            var didLog = false;
            LogDelegate appender = (log, logLevel, message) => didLog = true;
            fabl.AddAppender(appender);
            var logger = fabl.GetLogger("TestLogger");
            fabl.RemoveAppender(appender);
            logger.Info("test message");
            didLog.Should().BeFalse();
        }

        [Fact]
        public void ClearsGlobalAppenders()
        {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            fabl.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });
            fabl.ResetAppenders();
            var logger = fabl.GetLogger("TestLogger");
            logger.Info("test message");
            appenderLogLevel.Should().Be(LogLevel.Off);
            appenderMessage.Should().Be(string.Empty);
        }
    }
}
