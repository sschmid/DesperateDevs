using System;
using System.Text;

namespace DesperateDevs.Logging.Formatters {

    public class DefaultLogMessageFormatter {

        static readonly int _maxLogLevelLength;

        StringBuilder _stringBuilder = new StringBuilder();

        static DefaultLogMessageFormatter() {
            _maxLogLevelLength = getLongestLogLevelWordLength() + 2;
        }

        static int getLongestLogLevelWordLength() {
            var maxLength = 0;
            var names = Enum.GetNames(typeof(LogLevel));
            foreach (var name in names) {
                if (name.Length > maxLength) {
                    maxLength = name.Length;
                }
            }

            return maxLength;
        }

        public string FormatMessage(Logger logger, LogLevel logLevel, string message) {
            var logLevelStr = ("[" + logLevel.ToString().ToUpper() + "]").PadRight(_maxLogLevelLength);

            _stringBuilder.Length = 0;

            _stringBuilder.Append(logLevelStr);
            _stringBuilder.Append(" ");
            _stringBuilder.Append(logger.name);
            _stringBuilder.Append(": ");
            _stringBuilder.Append(message);

            return _stringBuilder.ToString();
        }
    }
}
