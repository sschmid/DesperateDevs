using System;
using System.Collections.Generic;

namespace DesperateDevs.Logging
{
    public static class Sherlog
    {
        public static LogLevel GlobalLogLevel
        {
            get => _globalLogLevel;
            set
            {
                _globalLogLevel = value;
                foreach (var logger in Loggers.Values)
                    logger.LogLevel = value;
            }
        }

        static LogLevel _globalLogLevel;
        static LogDelegate _appenders;
        static readonly Dictionary<string, Logger> Loggers = new Dictionary<string, Logger>();
        static readonly Logger Logger = GetLogger("Sherlog");

        public static void Trace(string message) => Logger.Trace(message);
        public static void Debug(string message) => Logger.Debug(message);
        public static void Info(string message) => Logger.Info(message);
        public static void Warn(string message) => Logger.Warn(message);
        public static void Error(string message) => Logger.Error(message);
        public static void Fatal(string message) => Logger.Fatal(message);

        public static void Assert(bool condition, string message) => Logger.Assert(condition, message);

        public static void AddAppender(LogDelegate appender)
        {
            _appenders += appender;
            foreach (var logger in Loggers.Values)
                logger.OnLog += appender;
        }

        public static void RemoveAppender(LogDelegate appender)
        {
            _appenders -= appender;
            foreach (var logger in Loggers.Values)
                logger.OnLog -= appender;
        }

        public static Logger GetLogger(Type type) => GetLogger(type.FullName);

        public static Logger GetLogger(string name)
        {
            if (!Loggers.TryGetValue(name, out var logger))
            {
                logger = new Logger(name)
                {
                    LogLevel = GlobalLogLevel
                };
                logger.OnLog += _appenders;
                Loggers.Add(name, logger);
            }

            return logger;
        }

        public static void ResetLoggers() => Loggers.Clear();
        public static void ResetAppenders() => _appenders = null;
    }
}
