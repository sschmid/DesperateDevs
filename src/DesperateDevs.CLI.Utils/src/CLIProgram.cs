using System;
using System.IO;
using System.Linq;
using DesperateDevs.Logging;
using DesperateDevs.Logging.Formatters;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;

namespace DesperateDevs.Cli.Utils
{
    public class CliProgram
    {
        readonly Logger _logger;
        readonly Type _defaultCommand;
        readonly string[] _args;
        readonly ICommand[] _commands;

        public CliProgram(string applicationName, Type defaultCommand, string[] args, ConsoleColors consoleColors = null)
        {
            _logger = Sherlog.GetLogger(applicationName);
            _defaultCommand = defaultCommand;
            _args = args;
            CliHelper.ConsoleColors = consoleColors ?? new ConsoleColors();
            Console.Title = applicationName + string.Join("  ", args);
            InitializeLogging(args, CliHelper.ConsoleColors);
            _commands = LoadCommands(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
        }

        public void Run()
        {
            if (_args?.WithoutParameter().Length != 0)
            {
                RunCommand(_args);
            }
            else
            {
                try
                {
                    _commands
                        .Single(c => c.GetType() == _defaultCommand)
                        .Run(this, _args);
                }
                catch (Exception exception)
                {
                    _logger.Error(_args.IsVerbose() ? exception.ToString() : exception.Message);
                }
            }
        }

        ICommand[] LoadCommands(string dir)
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

        public int GetCommandListPad() => _commands.Length == 0
            ? 0
            : _commands.Max(c => c.Example?.Length ?? 0);

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

        void RunCommand(string[] args)
        {
            try
            {
                GetCommand(args.WithoutDefaultParameter()[0]).Run(this, args);
            }
            catch (Exception exception)
            {
                _logger.Error(args.IsVerbose() ? exception.ToString() : exception.Message);
                _logger.Info("Use -v to enable verbose logging");
            }
        }

        void InitializeLogging(string[] args, ConsoleColors consoleColors)
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

            var formatter = args.IsDebug()
                ? (LogFormatter)new LogMessageFormatter().FormatMessage
                : (logger, level, message) => message;

            Sherlog.ResetAppenders();
            Sherlog.AddAppender((logger, logLevel, message) =>
            {
                message = formatter(logger, logLevel, message);
                if (consoleColors.LogLevelColors.ContainsKey(logLevel))
                {
                    Console.ForegroundColor = consoleColors.LogLevelColors[logLevel];
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
