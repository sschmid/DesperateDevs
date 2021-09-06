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
            fabl.globalLogLevel = LogLevel.On;
        }

        [Fact]
        public void CreatesNewLogger()
        {
            var logger = fabl.GetLogger("MyLogger");
            logger.Should().NotBeNull();
            logger.GetType().Should().BeSameAs(typeof(Logger));
            logger.name.Should().Be("MyLogger");
            logger.logLevel.Should().Be(LogLevel.On);
        }

        [Fact]
        public void ReturnsSameLoggerWhenNameIsEqual()
        {
            var logger1 = fabl.GetLogger("MyLogger");
            var logger2 = fabl.GetLogger("MyLogger");
            logger1.Should().BeSameAs(logger2);
        }

        [Fact]
        public void ClearsCreatedLoggers()
        {
            var logger1 = fabl.GetLogger("MyLogger");
            fabl.ResetLoggers();
            var logger2 = fabl.GetLogger("MyLogger");
            logger1.Should().NotBeSameAs(logger2);
        }

        [Fact]
        public void CreatesNewLoggerWithGlobalLogLevel()
        {
            fabl.globalLogLevel = LogLevel.Error;
            var logger = fabl.GetLogger("MyLogger");
            logger.logLevel.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void SetsGlobalLogLevelOnCreatedLogger()
        {
            var logger = fabl.GetLogger("MyLogger");
            logger.logLevel.Should().Be(LogLevel.On);
            fabl.globalLogLevel = LogLevel.Error;
            logger.logLevel.Should().Be(LogLevel.Error);
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

            var logger = fabl.GetLogger("MyLogger");
            logger.Info("hi");

            appenderLogLevel.Should().Be(LogLevel.Info);
            appenderMessage.Should().Be("hi");
            appenderLogLevel2.Should().Be(LogLevel.Info);
            appenderMessage2.Should().Be("hi");
        }

        [Fact]
        public void AddsAppenderOnCreatedLogger()
        {
            var logger = fabl.GetLogger("MyLogger");
            var didLog = false;
            fabl.AddAppender((log, logLevel, message) => didLog = true);
            logger.Info("hi");
            didLog.Should().BeTrue();
        }

        [Fact]
        public void RemovesAppenderOnCreatedLogger()
        {
            var didLog = false;
            LogDelegate appender = (log, logLevel, message) => didLog = true;
            fabl.AddAppender(appender);
            var logger = fabl.GetLogger("MyLogger");
            fabl.RemoveAppender(appender);
            logger.Info("hi");
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
            var logger = fabl.GetLogger("MyLogger");
            logger.Info("hi");
            appenderLogLevel.Should().Be(LogLevel.Off);
            appenderMessage.Should().Be(string.Empty);
        }
    }
}
