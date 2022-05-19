using System;
using System.Collections.Generic;
using Sherlog;

namespace DesperateDevs.Cli.Utils
{
    public class ConsoleColors
    {
        public ConsoleColor HighlightedBackground = ConsoleColor.White;
        public ConsoleColor HighlightedForeground = ConsoleColor.Black;

        public readonly Dictionary<LogLevel, ConsoleColor> LogLevelColors;

        public ConsoleColors()
        {
            LogLevelColors = new Dictionary<LogLevel, ConsoleColor>
            {
                {LogLevel.Trace, ConsoleColor.Cyan},
                {LogLevel.Debug, ConsoleColor.White},
                {LogLevel.Info, ConsoleColor.White},
                {LogLevel.Warn, ConsoleColor.DarkYellow},
                {LogLevel.Error, ConsoleColor.Red},
                {LogLevel.Fatal, ConsoleColor.DarkRed}
            };
        }
    }
}
