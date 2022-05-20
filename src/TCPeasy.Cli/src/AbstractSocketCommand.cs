﻿using System;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.Cli.Utils;
using Sherlog;

namespace TCPeasy.Cli
{
    public abstract class AbstractSocketCommand : AbstractCommand
    {
        protected readonly Logger _logger;
        protected AbstractTcpSocket _socket;

        TcpMessageParser _tcpMessageParser;

        protected AbstractSocketCommand(string loggerName)
        {
            _logger = Logger.GetLogger(loggerName);
        }

        protected void Start()
        {
            _tcpMessageParser = new TcpMessageParser();
            _tcpMessageParser.OnMessage += OnMessage;
            _socket.OnReceived += OnReceive;
            Console.CancelKeyPress += OnCancel;
            while (true)
            {
                _socket.Send(TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(Console.ReadLine() ?? string.Empty)));
            }
        }

        void OnMessage(TcpMessageParser messageParser, byte[] bytes) => _logger.Info(Encoding.UTF8.GetString(bytes));

        protected void OnReceive(AbstractTcpSocket socket, Socket client, byte[] bytes) => _tcpMessageParser.Receive(bytes);
        protected void OnCancel(object sender, ConsoleCancelEventArgs e) => _socket.Disconnect();
    }
}
