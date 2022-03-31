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
        public void CreatesNewLoggerForType()
        {
            var logger = Sherlog.GetLogger(typeof(SherlogTests));
            logger.Should().NotBeNull();
            logger.GetType().Should().BeSameAs(typeof(Logger));
            logger.Name.Should().Be("DesperateDevs.Logging.Tests.SherlogTests");
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
        public void ReturnsSameLoggerWhenTypeIsEqual()
        {
            var logger1 = Sherlog.GetLogger(typeof(SherlogTests));
            var logger2 = Sherlog.GetLogger(typeof(SherlogTests));
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

        [Fact]
        public void ForwardsToSherlogLogger()
        {
            Logger logger = null;
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            Sherlog.AddAppender((log, logLevel, message) =>
            {
                logger = log;
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });

            Sherlog.Trace("trace message");
            Sherlog.Trace("trace message");
            logger.Should().BeSameAs(Sherlog.GetLogger("Sherlog"));
            appenderLogLevel.Should().Be(LogLevel.Trace);
            appenderMessage.Should().Be("trace message");

            Sherlog.Debug("debug message");
            logger.Should().BeSameAs(Sherlog.GetLogger("Sherlog"));
            appenderLogLevel.Should().Be(LogLevel.Debug);
            appenderMessage.Should().Be("debug message");

            Sherlog.Info("info message");
            logger.Should().BeSameAs(Sherlog.GetLogger("Sherlog"));
            appenderLogLevel.Should().Be(LogLevel.Info);
            appenderMessage.Should().Be("info message");

            Sherlog.Warn("warn message");
            logger.Should().BeSameAs(Sherlog.GetLogger("Sherlog"));
            appenderLogLevel.Should().Be(LogLevel.Warn);
            appenderMessage.Should().Be("warn message");

            Sherlog.Error("error message");
            logger.Should().BeSameAs(Sherlog.GetLogger("Sherlog"));
            appenderLogLevel.Should().Be(LogLevel.Error);
            appenderMessage.Should().Be("error message");

            Sherlog.Fatal("fatal message");
            logger.Should().BeSameAs(Sherlog.GetLogger("Sherlog"));
            appenderLogLevel.Should().Be(LogLevel.Fatal);
            appenderMessage.Should().Be("fatal message");

            FluentActions.Invoking(() => Sherlog.Assert(false, "assert message"))
                .Should().Throw<SherlogAssertException>()
                .Which.Message.Should().Be("assert message");
        }
    }
}
