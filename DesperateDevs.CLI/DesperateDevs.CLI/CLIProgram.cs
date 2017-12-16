using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        readonly ICommand[] _commands;

        public CLIProgram(string applicationName) {
            _logger = fabl.GetLogger(applicationName);
            var basePath = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(basePath);
            var resolver = new AssemblyResolver(AppDomain.CurrentDomain, basePath);

            foreach (var file in files) {
                resolver.Load(file);
            }

            _commands = getOrderedCommands(resolver.GetTypes());
        }

        public void Run(string[] args, Action<ICommand[]> printUsage) {
            if (args == null || args.Length == 0) {
                printUsage(_commands);
                return;
            }

            initializeLogging(args, consoleColors);
            runCommand(args);
        }

        public ICommand GetCommand(string trigger) {
            var command = _commands.SingleOrDefault(c => c.trigger == trigger);
            if (command == null) {
                throw new Exception("command not found: " + trigger);
            }

            return command;
        }

        public int GetCommandListPad() {
            return _commands.Length == 0
                ? 0
                : _commands.Max(c => c.example != null ? c.example.Length : 0);
        }

        public List<string> GetFormattedCommandList() {
            var pad = GetCommandListPad();
            return _commands
                .Where(c => c.example != null)
                .Select(c => c.example.PadRight(pad) + " - " + c.description)
                .ToList();
        }

        void runCommand(string[] args) {
            try {
                GetCommand(args.WithoutDefaultParameter()[0]).Run(args);
            } catch (Exception ex) {
                _logger.Error(args.isVerbose() ? ex.ToString() : ex.Message);
            }
        }

        static ICommand[] getOrderedCommands(Type[] types) {
            return types
                .GetInstancesOf<ICommand>()
                .OrderBy(c => c.trigger)
                .ToArray();
        }

        void initializeLogging(string[] args, Dictionary<LogLevel, ConsoleColor> consoleColors) {
            fabl.globalLogLevel = args.isVerbose()
                ? LogLevel.On
                : LogLevel.Info;

            LogFormatter formatter;
            if (args.isDebug()) {
                formatter = new DefaultLogMessageFormatter().FormatMessage;
            } else {
                formatter = (logger, level, message) => message;
            }

            fabl.ResetAppenders();
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
    }
}
