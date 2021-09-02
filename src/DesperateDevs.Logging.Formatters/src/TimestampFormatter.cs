using System;

namespace DesperateDevs.Logging.Formatters {

    public class TimestampFormatter {

        readonly string _timeFormat;

        public TimestampFormatter() : this("{0:yyyy/MM/dd/hh:mm:ss:fff}") {
        }

        public TimestampFormatter(string timeFormat) {
            _timeFormat = timeFormat;
        }

        public string FormatMessage(Logger logger, LogLevel logLevel, string message) {
            var time = string.Format(_timeFormat, DateTime.Now);
            return string.Format(_timeFormat, time + " " + message);
        }
    }
}
