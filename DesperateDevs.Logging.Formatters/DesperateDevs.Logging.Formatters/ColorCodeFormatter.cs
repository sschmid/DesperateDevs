using System.Collections.Generic;
using System.Text;

namespace DesperateDevs.Logging.Formatters {

    public class ColorCodeFormatter {

        // ANSI COLOR escape codes for colors and other things.
        // You can change the color of foreground and background plus bold, italic, underline etc
        // For a complete list see http://en.wikipedia.org/wiki/ANSI_escape_code#Colors

        const string Reset = "0m";
        const string ESC = "\x1B[";

        // Foreground colors
        const string FG_Black   = "30m";
        const string FG_Red     = "31m";
        const string FG_Green   = "32m";
        const string FG_Yellow  = "33m";
        const string FG_Blue    = "34m";
        const string FG_Magenta = "35m";
        const string FG_Cyan    = "36m";
        const string FG_White   = "37m";

        // Background colors
        const string BG_None    = "";
        const string BG_Black   = "40m";
        const string BG_Red     = "41m";
        const string BG_Green   = "42m";
        const string BG_Yellow  = "43m";
        const string BG_Blue    = "44m";
        const string BG_Magenta = "45m";
        const string BG_Cyan    = "46m";
        const string BG_White   = "47m";


        static readonly Dictionary<LogLevel, string[]> colors = new Dictionary<LogLevel, string[]> {
            { LogLevel.Trace, new [] { FG_White,  BG_Cyan } },
            { LogLevel.Debug, new [] { FG_Blue,   BG_None } },
            { LogLevel.Info,  new [] { FG_Green,  BG_None } },
            { LogLevel.Warn,  new [] { FG_Yellow, BG_None } },
            { LogLevel.Error, new [] { FG_White,  BG_Red } },
            { LogLevel.Fatal, new [] { FG_White,  BG_Magenta } }
        };

        StringBuilder _stringBuilder = new StringBuilder();

        public string FormatMessage(Logger logger, LogLevel logLevel, string message) {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(ESC);
            _stringBuilder.Append(colors[logLevel][1]);

            _stringBuilder.Append(ESC);
            _stringBuilder.Append(colors[logLevel][0]);

            _stringBuilder.Append(message);

            _stringBuilder.Append(ESC);
            _stringBuilder.Append(Reset);

            return _stringBuilder.ToString();
        }
    }
}
