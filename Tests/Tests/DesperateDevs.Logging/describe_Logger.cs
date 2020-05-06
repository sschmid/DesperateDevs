﻿using System;
using DesperateDevs.Logging;
using NSpec;
using Shouldly;

class describe_Logger : nspec {

    Logger _logger;
    const string message = "hi";
    const string loggerName = "MyLogger";

    void when_created() {
        before = () => _logger = new Logger("MyLogger");
        doLogLevelTests();
        context["assert"] = () => {
            it["doesn't throw when condition is true"] = () => _logger.Assert(true, "works");
            it["throws when condition is false"] = expect<FablAssertException>(() => _logger.Assert(false, "doesn't work"));
        };

        it["resets OnLog"] = () => {
            var didLog = 0;
            _logger.OnLog += (logger, level, s) => didLog += 1;
            _logger.Reset();
            _logger.Info("Test");
            didLog.ShouldBe(0);
        };
    }

    void doLogLevelTests() {
        testAllLogLevels(LogLevel.On,    true,  true,  true,  true,  true,  true);
        testAllLogLevels(LogLevel.Trace, true,  true,  true,  true,  true,  true);
        testAllLogLevels(LogLevel.Debug, false, true,  true,  true,  true,  true);
        testAllLogLevels(LogLevel.Info,  false, false, true,  true,  true,  true);
        testAllLogLevels(LogLevel.Warn,  false, false, false, true,  true,  true);
        testAllLogLevels(LogLevel.Error, false, false, false, false, true,  true);
        testAllLogLevels(LogLevel.Fatal, false, false, false, false, false, true);
        testAllLogLevels(LogLevel.Off,   false, false, false, false, false, false);
    }

    void testAllLogLevels(LogLevel logLevel, bool trace, bool debug, bool info, bool warn, bool error, bool fatal) {
        context[logLevel.ToString()] = () => {
            before = () => _logger.logLevel = logLevel;
            it[logs(trace) + " trace"] = () => testLogLevel(_logger.Trace, LogLevel.Trace, trace);
            it[logs(debug) + " debug"] = () => testLogLevel(_logger.Debug, LogLevel.Debug, debug);
            it[logs(info)  + " info"]  = () => testLogLevel(_logger.Info,  LogLevel.Info,  info);
            it[logs(warn)  + " warn"]  = () => testLogLevel(_logger.Warn,  LogLevel.Warn,  warn);
            it[logs(error) + " error"] = () => testLogLevel(_logger.Error, LogLevel.Error, error);
            it[logs(fatal) + " fatal"] = () => testLogLevel(_logger.Fatal, LogLevel.Fatal, fatal);
        };
    }

    string logs(bool value) {
        return value ? "logs" : "doesn't log";
    }

    void testLogLevel(Action<string> logMethod, LogLevel logLvl, bool shouldLog) {
        var didLog = false;
        LogLevel eventLogLevel = LogLevel.Off;
        string eventMessage = null;
        Logger eventLogger = null;
        _logger.OnLog += (logger, logLevel, msg) => {
            didLog = true;
            eventLogger = logger;
            eventLogLevel = logLevel;
            eventMessage = msg;
        };

        logMethod(message);

        didLog.ShouldBe(shouldLog);

        if (shouldLog) {
            eventLogger.ShouldBeSameAs(_logger);
            eventMessage.ShouldBe(message);
            eventLogLevel.ShouldBe(logLvl);
        } else {
            eventMessage.ShouldBeNull();
            eventLogLevel.ShouldBe(LogLevel.Off);
            eventLogger.ShouldBeNull();
        }
    }
}
