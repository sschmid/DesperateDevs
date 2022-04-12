using System;
using System.IO;
using System.Linq;
using DesperateDevs.Logging;
using DesperateDevs.Logging.Formatters;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;

namespace DesperateDevs.CLI.Utils
{
    public class CLIProgram
    {
        readonly Logger _logger;
        readonly Type _defaultCommand;
        readonly string[] _args;
        readonly ICommand[] _commands;

        public CLIProgram(string applicationName, Type defaultCommand, string[] args, ConsoleColors consoleColors = null)
        {
            _logger = Sherlog.GetLogger(applicationName);
            _defaultCommand = defaultCommand;
            _args = args;
            CLIHelper.consoleColors = consoleColors ?? new ConsoleColors();
            Console.Title = applicationName + string.Join("  ", args);
            initializeLogging(args, CLIHelper.consoleColors);
            _commands = loadCommands(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
        }

        public void Run()
        {
            if (_args != null && _args.WithoutParameter().Length != 0)
            {
                runCommand(_args);
            }
            else
            {
                try
                {
                    _commands
                        .Single(c => c.GetType() == _defaultCommand)
                        .Run(this, _args);
                }
                catch (Exception ex)
                {
                    _logger.Error(_args.IsVerbose() ? ex.ToString() : ex.Message);
                }
            }
        }

        ICommand[] loadCommands(string dir)
        {
            _logger.Debug("Loading assemblies from " + dir);
            var resolver = AssemblyResolver.LoadAssemblies(false, dir);

            var commands = AppDomain.CurrentDomain
                .GetInstancesOf<ICommand>()
                .OrderBy(c => c.Trigger)
                .ToArray();

            resolver.Close();

            return commands;
        }

        public ICommand GetCommand(string trigger)
        {
            var command = _commands.SingleOrDefault(c => c.Trigger == trigger);
            if (command == null)
            {
                throw new Exception("command not found: " + trigger);
            }

            return command;
        }

        public int GetCommandListPad()
        {
            return _commands.Length == 0
                ? 0
                : _commands.Max(c => c.Example?.Length ?? 0);
        }

        public string GetFormattedCommandList()
        {
            var pad = GetCommandListPad();

            var groupedCommands = _commands
                .Where(c => c.Example != null)
                .GroupBy(c => c.Group ?? string.Empty)
                .OrderBy(group => group.Key);

            return string.Join("\n", groupedCommands.Select(group =>
            {
                var groupHeader = group.Key == string.Empty ? string.Empty : group.Key + ":\n";
                var commandInGroup = string.Join("\n", group
                    .Select(command => "  " + command.Example.PadRight(pad) + " - " + command.Description));
                return groupHeader + "\n" + commandInGroup + "\n";
            }));
        }

        void runCommand(string[] args)
        {
            try
            {
                GetCommand(args.WithoutDefaultParameter()[0]).Run(this, args);
            }
            catch (Exception ex)
            {
                _logger.Error(args.IsVerbose() ? ex.ToString() : ex.Message);
                _logger.Info("Use -v to enable verbose logging");
            }
        }

        void initializeLogging(string[] args, ConsoleColors consoleColors)
        {
            if (args.IsSilent())
            {
                Sherlog.GlobalLogLevel = LogLevel.Error;
            }
            else if (args.IsVerbose())
            {
                Sherlog.GlobalLogLevel = LogLevel.Debug;
            }
            else
            {
                Sherlog.GlobalLogLevel = LogLevel.Info;
            }

            LogFormatter formatter;
            if (args.IsDebug())
            {
                formatter = new LogMessageFormatter().FormatMessage;
            }
            else
            {
                formatter = (logger, level, message) => message;
            }

            Sherlog.ResetAppenders();
            Sherlog.AddAppender((logger, logLevel, message) =>
            {
                message = formatter(logger, logLevel, message);
                if (consoleColors.logLevelColors.ContainsKey(logLevel))
                {
                    Console.ForegroundColor = consoleColors.logLevelColors[logLevel];
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(message);
                }
            });
        }
    }
}
