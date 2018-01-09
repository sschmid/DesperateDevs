using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DesperateDevs.Logging;
using DesperateDevs.Logging.Formatters;
using DesperateDevs.Utils;

namespace DesperateDevs.CLI.Utils {

    public class CLIProgram {

        public static readonly Dictionary<LogLevel, ConsoleColor> consoleColors =
            new Dictionary<LogLevel, ConsoleColor> {
                { LogLevel.Warn, ConsoleColor.DarkYellow },
                { LogLevel.Error, ConsoleColor.Red },
                { LogLevel.Fatal, ConsoleColor.DarkRed }
            };

        readonly Logger _logger;
        readonly ICommand[] _commands;

        public CLIProgram(string applicationName, string[] args, Action<ICommand[]> printUsage) {
            _logger = fabl.GetLogger(applicationName);
            initializeLogging(args, consoleColors);

            var path = Assembly.GetExecutingAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(path);
            _logger.Debug("Loading commands from " + assemblyDir);
            var resolver = AssemblyResolver.LoadAssembliesContainingType<ICommand>(false, assemblyDir);

            _commands = AppDomain.CurrentDomain
                .GetInstancesOf<ICommand>()
                .OrderBy(c => c.trigger)
                .ToArray();

            resolver.Close();

            if (args == null || args.WithoutParameter().Length == 0) {
                printUsage(_commands);
                return;
            }

            runCommand(args);
        }

        public ICommand GetCommand(string trigger) {
            var command = _commands.SingleOrDefault(c => c.trigger == trigger);
            if (command == null) {
                throw new Exception("command not found: " + trigger);
            }

            return command;
        }

        public static int GetCommandListPad(ICommand[] commands) {
            return commands.Length == 0
                ? 0
                : commands.Max(c => c.example != null ? c.example.Length : 0);
        }

        public static List<string> GetFormattedCommandList(ICommand[] commands) {
            var pad = GetCommandListPad(commands);
            return commands
                .Where(c => c.example != null)
                .Select(c => c.example.PadRight(pad) + " - " + c.description)
                .ToList();
        }

        void runCommand(string[] args) {
            try {
                GetCommand(args.WithoutDefaultParameter()[0]).Run(args);
            } catch (Exception ex) {
                _logger.Error(args.IsVerbose() ? ex.ToString() : ex.Message);
            }
        }

        void initializeLogging(string[] args, Dictionary<LogLevel, ConsoleColor> consoleColors) {
            fabl.globalLogLevel = args.IsVerbose()
                ? LogLevel.On
                : LogLevel.Info;

            LogFormatter formatter;
            if (args.IsDebug()) {
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
