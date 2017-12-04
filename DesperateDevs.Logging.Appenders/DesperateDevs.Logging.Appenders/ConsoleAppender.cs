using System.Collections.Generic;
using System;

namespace DesperateDevs.Logging.Appenders {

    public class ConsoleAppender {

        readonly Dictionary<LogLevel, ConsoleColor> _consoleColors;

        public ConsoleAppender() : this(new Dictionary<LogLevel, ConsoleColor> {
            { LogLevel.Trace, ConsoleColor.Cyan },
            { LogLevel.Warn, ConsoleColor.DarkYellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.DarkRed }
        }) {
        }

        public ConsoleAppender(Dictionary<LogLevel, ConsoleColor> consoleColors) {
            _consoleColors = consoleColors;
        }

        public void WriteLine(Logger logger, LogLevel logLevel, string message) {
            ConsoleColor color;
            if (_consoleColors.TryGetValue(logLevel, out color)) {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            } else {
                Console.WriteLine(message);
            }
        }
    }
}
