using FluentAssertions;
using Xunit;

namespace DesperateDevs.Logging.Tests
{
    public class SherlogTests
    {
        public SherlogTests()
        {
            Sherlog.ResetLoggers();
            Sherlog.ResetAppenders();
            Sherlog.GlobalLogLevel = LogLevel.On;
        }

        [Fact]
        public void CreatesNewLogger()
        {
            var logger = Sherlog.GetLogger("TestLogger");
            logger.Should().NotBeNull();
            logger.GetType().Should().BeSameAs(typeof(Logger));
            logger.Name.Should().Be("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.On);
        }

        [Fact]
        public void ReturnsSameLoggerWhenNameIsEqual()
        {
            var logger1 = Sherlog.GetLogger("TestLogger");
            var logger2 = Sherlog.GetLogger("TestLogger");
            logger1.Should().BeSameAs(logger2);
        }

        [Fact]
        public void ClearsCreatedLoggers()
        {
            var logger1 = Sherlog.GetLogger("TestLogger");
            Sherlog.ResetLoggers();
            var logger2 = Sherlog.GetLogger("TestLogger");
            logger1.Should().NotBeSameAs(logger2);
        }

        [Fact]
        public void CreatesNewLoggerWithGlobalLogLevel()
        {
            Sherlog.GlobalLogLevel = LogLevel.Error;
            var logger = Sherlog.GetLogger("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void SetsGlobalLogLevelOnCreatedLogger()
        {
            var logger = Sherlog.GetLogger("TestLogger");
            logger.LogLevel.Should().Be(LogLevel.On);
            Sherlog.GlobalLogLevel = LogLevel.Error;
            logger.LogLevel.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void CreatesNewLoggerWithGlobalAppender()
        {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            Sherlog.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });

            var appenderLogLevel2 = LogLevel.Off;
            var appenderMessage2 = string.Empty;
            Sherlog.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel2 = logLevel;
                appenderMessage2 = message;
            });

            var logger = Sherlog.GetLogger("TestLogger");
            logger.Info("test message");

            appenderLogLevel.Should().Be(LogLevel.Info);
            appenderMessage.Should().Be("test message");
            appenderLogLevel2.Should().Be(LogLevel.Info);
            appenderMessage2.Should().Be("test message");
        }

        [Fact]
        public void AddsAppenderOnCreatedLogger()
        {
            var logger = Sherlog.GetLogger("TestLogger");
            var didLog = false;
            Sherlog.AddAppender((log, logLevel, message) => didLog = true);
            logger.Info("test message");
            didLog.Should().BeTrue();
        }

        [Fact]
        public void RemovesAppenderOnCreatedLogger()
        {
            var didLog = false;
            LogDelegate appender = (log, logLevel, message) => didLog = true;
            Sherlog.AddAppender(appender);
            var logger = Sherlog.GetLogger("TestLogger");
            Sherlog.RemoveAppender(appender);
            logger.Info("test message");
            didLog.Should().BeFalse();
        }

        [Fact]
        public void ClearsGlobalAppenders()
        {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            Sherlog.AddAppender((log, logLevel, message) =>
            {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });
            Sherlog.ResetAppenders();
            var logger = Sherlog.GetLogger("TestLogger");
            logger.Info("test message");
            appenderLogLevel.Should().Be(LogLevel.Off);
            appenderMessage.Should().Be(string.Empty);
        }
    }
}
