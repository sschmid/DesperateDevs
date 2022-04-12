﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.Cli.Utils;
using DesperateDevs.Logging;
using DesperateDevs.Net;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class ServerCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "server";
        public override string Description => "Start server mode";
        public override string Group => CommandGroups.CODE_GENERATION;
        public override string Example => "server";

        AbstractTcpSocket _socket;
        readonly List<string> _logBuffer = new List<string>();

        public ServerCommand() : base(typeof(ServerCommand).FullName)
        {
        }

        protected override void Run()
        {
            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var server = new TcpServerSocket();
            _socket = server;
            server.OnReceived += onReceived;
            server.Listen(config.port);
            Console.CancelKeyPress += onCancel;
            while (true)
            {
                server.Send(Encoding.UTF8.GetBytes(Console.ReadLine()));
            }
        }

        void onReceived(AbstractTcpSocket socket, Socket client, byte[] bytes)
        {
            var message = Encoding.UTF8.GetString(bytes);
            _logger.Info(message);

            var args = getArgsFromMessage(message);

            try
            {
                if (args[0] == Trigger)
                {
                    throw new Exception("Server is already running!");
                }
                var command = _program.GetCommand(args.WithoutDefaultParameter()[0]);
                Sherlog.AddAppender(onLog);
                command.Run(_program, args);
                Sherlog.RemoveAppender(onLog);
                var logBufferString = getLogBufferString();
                var sendBytes = logBufferString.Length == 0
                    ? new byte[] { 0 }
                    : Encoding.UTF8.GetBytes(logBufferString);
                socket.Send(sendBytes);
            }
            catch (Exception ex)
            {
                _logger.Error(args.IsVerbose()
                    ? ex.ToString()
                    : ex.Message);

                socket.Send(Encoding.UTF8.GetBytes(getLogBufferString() + ex.Message));
            }

            _logBuffer.Clear();
        }

        string[] getArgsFromMessage(string command)
        {
            return command
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }

        void onCancel(object sender, ConsoleCancelEventArgs e)
        {
            _socket.Disconnect();
        }

        string getLogBufferString()
        {
            return string.Join("\n", _logBuffer.ToArray());
        }

        void onLog(Logger logger, LogLevel loglevel, string message)
        {
            _logBuffer.Add(message);
        }
    }
}
