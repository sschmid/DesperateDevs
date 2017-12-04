using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.CLI;
using DesperateDevs.Logging;
using DesperateDevs.Networking;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Server : AbstractPreferencesCommand {

        public override string trigger { get { return "server"; } }
        public override string description { get { return "Start server mode (default port is 3333)"; } }
        public override string example { get { return "jenny server port"; } }

        AbstractTcpSocket _socket;
        readonly List<string> _logBuffer = new List<string>();

        public Server() : base(typeof(Server).Name) {
        }

        protected override void run() {
            var port = 0;
            try {
                port = int.Parse(_args[0]);
            } catch (Exception) {
                port = 3333;
            }

            var server = new TcpServerSocket();
            _socket = server;
            server.OnReceived += onReceived;
            server.Listen(port);
            Console.CancelKeyPress += onCancel;
            while (true) {
                server.Send(Encoding.Unicode.GetBytes(Console.ReadLine()));
            }
        }

        void onReceived(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            var message = Encoding.Unicode.GetString(bytes);
            _logger.Info(message);

            var args = getArgsFromMessage(message);

            try {
                if (args[0] == trigger) {
                    throw new Exception("Server is already running!");
                }
                var command = Program.GetCommand(args.WithoutParameter()[0]);
                fabl.AddAppender(onLog);
                command.Run(args);
                fabl.RemoveAppender(onLog);
                var logBufferString = getLogBufferString();
                var sendBytes = logBufferString.Length == 0
                    ? new byte[] { 0 }
                    : Encoding.Unicode.GetBytes(logBufferString);
                socket.Send(sendBytes);
            } catch (Exception ex) {
                _logger.Error(args.isVerbose()
                    ? ex.ToString()
                    : ex.Message);

                socket.Send(Encoding.Unicode.GetBytes(getLogBufferString() + ex.Message));
            }

            _logBuffer.Clear();
        }

        string[] getArgsFromMessage(string command) {
            return command
                .Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }

        void onCancel(object sender, ConsoleCancelEventArgs e) {
            _socket.Disconnect();
        }

        string getLogBufferString() {
            return string.Join("\n", _logBuffer.ToArray());
        }

        void onLog(Logger logger, LogLevel loglevel, string message) {
            _logBuffer.Add(message);
        }
    }
}
