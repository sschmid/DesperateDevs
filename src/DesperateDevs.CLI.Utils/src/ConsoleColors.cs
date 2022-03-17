using System;
using System.Collections.Generic;
using DesperateDevs.Logging;

namespace DesperateDevs.CLI.Utils
{
    public class ConsoleColors
    {
        public ConsoleColor highlightedBackground { get; set; } = ConsoleColor.White;
        public ConsoleColor highlightedForeground { get; set; } = ConsoleColor.Black;

        public readonly Dictionary<LogLevel, ConsoleColor> logLevelColors;

        public ConsoleColors()
        {
            logLevelColors = new Dictionary<LogLevel, ConsoleColor> {
                { LogLevel.Trace, ConsoleColor.Cyan },
                { LogLevel.Debug, ConsoleColor.White },
                { LogLevel.Info, ConsoleColor.White },
                { LogLevel.Warn, ConsoleColor.DarkYellow },
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.DarkRed }
            };
        }
    }
}
