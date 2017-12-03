﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DesperateDevs.Logging;
using DesperateDevs.Logging.Formatters;
using DesperateDevs.Utils;

namespace DesperateDevs.CLI {

    public class CLIProgram {

        public readonly Dictionary<LogLevel, ConsoleColor> consoleColors =
            new Dictionary<LogLevel, ConsoleColor> {
                { LogLevel.Warn, ConsoleColor.DarkYellow },
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.DarkRed }
            };

        readonly Logger _logger;

        public CLIProgram(string loggerName) {
            _logger = fabl.GetLogger(loggerName);
        }

        public static int GetCommandListPad(ICommand[] commands) {
            return commands.Max(c => c.example.Length);
        }

        public static List<string> GetFormattedCommandList(ICommand[] commands) {
            var pad = GetCommandListPad(commands);
            return commands
                .Select(c => c.example.PadRight(pad) + " - " + c.description)
                .ToList();
        }

        public void Run(string[] args, Assembly assembly, Action<ICommand[]> printUsage) {
            var commands = getOrderedCommands(assembly);

            if (args == null || args.Length == 0) {
                printUsage(commands);
                return;
            }

            initializeLogging(args, consoleColors);
            runCommand(commands, args);
        }

        void runCommand(ICommand[] commands, string[] args) {
            try {
                getCommand(commands, args[0]).Run(args);
            } catch (Exception ex) {
                _logger.Error(args.isVerbose() ? ex.ToString() : ex.Message);
            }
        }

        static ICommand[] getOrderedCommands(Assembly assembly) {
            return assembly
                .GetTypes()
                .GetInstancesOf<ICommand>()
                .OrderBy(c => c.trigger)
                .ToArray();
        }

        static void initializeLogging(string[] args, Dictionary<LogLevel, ConsoleColor> consoleColors) {
            fabl.globalLogLevel = (args.isVerbose())
                ? LogLevel.On
                : LogLevel.Info;

            LogFormatter formatter;
            if (args.isDebug()) {
                formatter = new DefaultLogMessageFormatter().FormatMessage;
            } else {
                formatter = (logger, level, message) => message;
            }

            fabl.AddAppender((logger, logLevel, message) => {
                message = formatter(logger, logLevel, message);
                if (consoleColors.ContainsKey(logLevel)) {
                    Console.ForegroundColor = consoleColors[logLevel];
                    Console.WriteLine(message);
                    Console.ResetColor();
                } else {
                    Console.WriteLine(message);
                }
            });
        }

        static ICommand getCommand(ICommand[] commands, string trigger) {
            var command = commands.SingleOrDefault(c => c.trigger == trigger);
            if (command == null) {
                throw new Exception("command not found: " + trigger);
            }

            return command;
        }
    }
}
