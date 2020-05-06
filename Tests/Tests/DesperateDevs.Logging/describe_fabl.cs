using DesperateDevs.Logging;
using NSpec;
using Shouldly;

class describe_fabl : nspec {

    void context_LoggerFactory() {

        before = () => {
            fabl.ResetLoggers();
            fabl.ResetAppenders();
            fabl.globalLogLevel = LogLevel.On;
        };

        it["creates a new logger"] = () => {
            var logger = fabl.GetLogger("MyLogger");
            logger.ShouldNotBeNull();
            logger.GetType().ShouldBeSameAs(typeof(Logger));
            logger.name.ShouldBe("MyLogger");
            logger.logLevel.ShouldBe(LogLevel.On);
        };

        it["returns same logger when name is equal"] = () => {
            var logger1 = fabl.GetLogger("MyLogger");
            var logger2 = fabl.GetLogger("MyLogger");
            logger1.ShouldBeSameAs(logger2);
        };

        it["clears created loggers"] = () => {
            var logger1 = fabl.GetLogger("MyLogger");
            fabl.ResetLoggers();
            var logger2 = fabl.GetLogger("MyLogger");
            logger1.ShouldNotBeSameAs(logger2);
        };

        it["creates a new logger with globalLogLevel"] = () => {
            fabl.globalLogLevel = LogLevel.Error;
            var logger = fabl.GetLogger("MyLogger");
            logger.logLevel.ShouldBe(LogLevel.Error);
        };

        it["sets global logLevel on created logger"] = () => {
            var logger = fabl.GetLogger("MyLogger");
            logger.logLevel.ShouldBe(LogLevel.On);
            fabl.globalLogLevel = LogLevel.Error;
            logger.logLevel.ShouldBe(LogLevel.Error);
        };

        it["creates new logger with global appender"] = () => {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            fabl.AddAppender((log, logLevel, message) => {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });

            var appenderLogLevel2 = LogLevel.Off;
            var appenderMessage2 = string.Empty;
            fabl.AddAppender((log, logLevel, message) => {
                appenderLogLevel2 = logLevel;
                appenderMessage2 = message;
            });

            var logger = fabl.GetLogger("MyLogger");
            logger.Info("hi");

            appenderLogLevel.ShouldBe(LogLevel.Info);
            appenderMessage.ShouldBe("hi");
            appenderLogLevel2.ShouldBe(LogLevel.Info);
            appenderMessage2.ShouldBe("hi");
        };

        it["adds appender on created logger"] = () => {
            var logger = fabl.GetLogger("MyLogger");
            var didLog = false;
            fabl.AddAppender((log, logLevel, message) => didLog = true);
            logger.Info("hi");
            didLog.ShouldBeTrue();
        };

        it["removes appender on created logger"] = () => {
            var didLog = false;
            LogDelegate appender = (log, logLevel, message) => didLog = true;
            fabl.AddAppender(appender);
            var logger = fabl.GetLogger("MyLogger");
            fabl.RemoveAppender(appender);
            logger.Info("hi");
            didLog.ShouldBeFalse();
        };

        it["clears global appenders"] = () => {
            var appenderLogLevel = LogLevel.Off;
            var appenderMessage = string.Empty;
            fabl.AddAppender((log, logLevel, message) => {
                appenderLogLevel = logLevel;
                appenderMessage = message;
            });
            fabl.ResetAppenders();
            var logger = fabl.GetLogger("MyLogger");
            logger.Info("hi");
            appenderLogLevel.ShouldBe(LogLevel.Off);
            appenderMessage.ShouldBe(string.Empty);
        };
    }
}
