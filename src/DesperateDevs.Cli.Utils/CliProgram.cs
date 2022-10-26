using System;
using System.IO;
using System.Linq;
using DesperateDevs.Reflection;
using Sherlog;
using Sherlog.Formatters;

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
            _logger = Logger.GetLogger(applicationName);
            _defaultCommand = defaultCommand;
            _args = args;
            CliHelper.ConsoleColors = consoleColors ?? new ConsoleColors();
            Console.Title = $"{applicationName} {string.Join("  ", args)}";
            InitializeLogging(args, CliHelper.ConsoleColors);
            _commands = LoadCommands(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
        }

        public void Run()
        {
            if (_args?.WithoutParameter().Any() ?? false)
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
            _logger.Debug($"Loading assemblies from {dir}");
            using (AssemblyResolver.LoadAssemblies(false, dir))
            {
                return AppDomain.CurrentDomain
                    .GetInstancesOf<ICommand>()
                    .OrderBy(c => c.Trigger)
                    .ToArray();
            }
        }

        void RunCommand(string[] args)
        {
            try
            {
                GetCommand(args.WithoutDefaultParameter().First()).Run(this, args);
            }
            catch (Exception exception)
            {
                _logger.Error(args.IsVerbose() ? exception.ToString() : exception.Message);
                _logger.Info("Use -v to enable verbose logging");
            }
        }

        public ICommand GetCommand(string trigger)
        {
            var command = _commands.SingleOrDefault(c => c.Trigger == trigger);
            if (command == null)
                throw new Exception($"command not found: {trigger}");

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
                var groupHeader = group.Key == string.Empty ? string.Empty : $"{group.Key}:\n";
                var commandsInGroup = string.Join("\n", group
                    .Select(command => $"  {command.Example.PadRight(pad)}   {command.Description}"));
                return $"{groupHeader}\n{commandsInGroup}\n";
            }));
        }

        void InitializeLogging(string[] args, ConsoleColors consoleColors)
        {
            if (args.IsSilent()) Logger.GlobalLogLevel = LogLevel.Error;
            else if (args.IsVerbose()) Logger.GlobalLogLevel = LogLevel.Debug;
            else Logger.GlobalLogLevel = LogLevel.Info;

            var formatter = args.IsDebug()
                ? (LogFormatter)new LogMessageFormatter().FormatMessage
                : (_, _, message) => message;

            Logger.AddAppender((logger, logLevel, message) =>
            {
                message = formatter(logger, logLevel, message);
                if (consoleColors.LogLevelColors.TryGetValue(logLevel, out var color))
                {
                    Console.ForegroundColor = color;
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
