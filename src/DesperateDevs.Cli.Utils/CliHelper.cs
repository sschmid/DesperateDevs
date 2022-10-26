using System;
using System.Linq;

namespace DesperateDevs.Cli.Utils
{
    public static class CliHelper
    {
        public static ConsoleColors ConsoleColors;

        public static void WriteCentered(string value, bool centerEachLine)
        {
            WriteMultiLine(value, centerEachLine, (line, length) =>
            {
                Console.SetCursorPosition(Math.Max(0, (Console.BufferWidth - length) / 2), Console.CursorTop);
                Console.WriteLine(line);
            });
        }

        public static void WriteCenteredHighlighted(string value, bool centerEachLine)
        {
            Console.BackgroundColor = ConsoleColors.HighlightedBackground;
            Console.ForegroundColor = ConsoleColors.HighlightedForeground;

            WriteMultiLine(value, centerEachLine, (line, length) =>
            {
                Console.SetCursorPosition(Math.Max(0, (Console.BufferWidth - length) / 2), Console.CursorTop);
                Console.WriteLine(line);
            });

            Console.ResetColor();
        }

        public static void WriteCenteredHighlightedPadded(string value, bool centerEachLine)
        {
            Console.BackgroundColor = ConsoleColors.HighlightedBackground;
            Console.ForegroundColor = ConsoleColors.HighlightedForeground;

            WriteMultiLine(value, centerEachLine, (line, length) =>
            {
                var pad = Math.Max(0, (Console.BufferWidth - length) / 2);
                Console.WriteLine(line.PadLeft(line.Length + pad).PadRight(Console.BufferWidth - Console.CursorLeft - 1));
            });

            Console.ResetColor();
        }

        public static void WriteRight(string value, bool centerEachLine)
        {
            WriteMultiLine(value, centerEachLine, (line, length) =>
            {
                Console.SetCursorPosition(Math.Max(0, Console.BufferWidth - length), Console.CursorTop);
                Console.Write(line);
            });
        }

        public static void WriteRightHighlighted(string value, bool centerEachLine)
        {
            Console.BackgroundColor = ConsoleColors.HighlightedBackground;
            Console.ForegroundColor = ConsoleColors.HighlightedForeground;

            WriteMultiLine(value, centerEachLine, (line, length) =>
            {
                Console.SetCursorPosition(Math.Max(0, Console.BufferWidth - length), Console.CursorTop);
                Console.Write(line);
            });

            Console.ResetColor();
        }

        public static void WriteRightHighlightedPadded(string value, bool centerEachLine)
        {
            Console.BackgroundColor = ConsoleColors.HighlightedBackground;
            Console.ForegroundColor = ConsoleColors.HighlightedForeground;

            WriteMultiLine(value, centerEachLine, (line, length) =>
            {
                var pad = Math.Max(0, Console.BufferWidth - length);
                Console.Write(line.PadLeft(length + pad));
            });

            Console.ResetColor();
        }

        public static void WriteHighlighted(string value, bool padRight, int pad = -1)
        {
            Console.BackgroundColor = ConsoleColors.HighlightedBackground;
            Console.ForegroundColor = ConsoleColors.HighlightedForeground;

            WriteMultiLine(value, true, (line, length) => Console.WriteLine(padRight
                ? line.PadRight(pad == -1 ? (Console.BufferWidth - Console.CursorLeft) : pad)
                : line));

            Console.ResetColor();
        }

        static void WriteMultiLine(string value, bool lengthPerLine, Action<string, int> logMethod)
        {
            var lines = value.Split('\n');
            if (lengthPerLine)
            {
                foreach (var line in lines)
                    logMethod(line, line.Length);
            }
            else
            {
                var longestLine = lines.Max(line => line.Length);
                foreach (var line in lines)
                    logMethod(line, longestLine);
            }
        }

//        public static void TestWrite()
//        {
//            Console.Clear();
//            WriteCentered("xxx", false);
//            WriteCentered("xxx", true);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//
//            WriteCentered("WriteCentered", false);
//            WriteCentered("WriteCentered\nper line", true);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//
//            WriteCenteredHighlightedPadded("WriteCenteredHighlightedPadded", false);
//            WriteCenteredHighlightedPadded("WriteCenteredHighlightedPadded\nper line", true);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//
//            WriteCenteredHighlighted("WriteCenteredHighlighted", false);
//            WriteCenteredHighlighted("WriteCenteredHighlighted\nper line", true);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//
//            WriteRight("WriteRight", false);
//            WriteRight("WriteRight\nper line", true);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//
//            WriteRightHighlightedPadded("WriteRightHighlightedPadded", false);
//            WriteRightHighlightedPadded("WriteRightHighlightedPadded\nper line", true);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//
//            WriteRightHighlighted("WriteRightHighlighted", false);
//            WriteRightHighlighted("WriteRightHighlighted\nper line", true);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//
//            WriteHighlighted("WriteHighlighted", false);
//            WriteHighlighted("WriteHighlighted\nPadded", true);
//            WriteHighlighted("WriteHighlighted\nCustom Pad", true, 40);
//
//            Console.WriteLine("--------------------------------------------------------------------------------");
//        }
    }
}
