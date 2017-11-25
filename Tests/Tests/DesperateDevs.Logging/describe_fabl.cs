using DesperateDevs.Logging;
using NSpec;

class describe_fabl : nspec {

    void context_LoggerFactory() {

        before = () => {
            fabl.ResetLoggers();
            fabl.ResetAppenders();
            fabl.globalLogLevel = LogLevel.On;
        };

        it["creates a new logger"] = () => {
            var logger = fabl.GetLogger("MyLogger");
            logger.should_not_be_null();
            logger.GetType().should_be_same(typeof(Logger));
            logger.name.should_be("MyLogger");
            logger.logLevel.should_be(LogLevel.On);
        };

        it["returns same logger when name is equal"] = () => {
            var logger1 = fabl.GetLogger("MyLogger");
            var logger2 = fabl.GetLogger("MyLogger");
            logger1.should_be_same(logger2);
        };

        it["clears created loggers"] = () => {
            var logger1 = fabl.GetLogger("MyLogger");
            fabl.ResetLoggers();
            var logger2 = fabl.GetLogger("MyLogger");
            logger1.should_not_be_same(logger2);
        };

        it["creates a new logger with globalLogLevel"] = () => {
            fabl.globalLogLevel = LogLevel.Error;
            var logger = fabl.GetLogger("MyLogger");
            logger.logLevel.should_be(LogLevel.Error);
        };

        it["sets global logLevel on created logger"] = () => {
            var logger = fabl.GetLogger("MyLogger");
            logger.logLevel.should_be(LogLevel.On);
            fabl.globalLogLevel = LogLevel.Error;
            logger.logLevel.should_be(LogLevel.Error);
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

            appenderLogLevel.should_be(LogLevel.Info);
            appenderMessage.should_be("hi");
            appenderLogLevel2.should_be(LogLevel.Info);
            appenderMessage2.should_be("hi");
        };

        it["adds appender on created logger"] = () => {
            var logger = fabl.GetLogger("MyLogger");
            var didLog = false;
            fabl.AddAppender((log, logLevel, message) => didLog = true);
            logger.Info("hi");
            didLog.should_be_true();
        };

        it["removes appender on created logger"] = () => {
            var didLog = false;
            LogDelegate appender = (log, logLevel, message) => didLog = true;
            fabl.AddAppender(appender);
            var logger = fabl.GetLogger("MyLogger");
            fabl.RemoveAppender(appender);
            logger.Info("hi");
            didLog.should_be_false();
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
            appenderLogLevel.should_be(LogLevel.Off);
            appenderMessage.should_be(string.Empty);
        };
    }
}
