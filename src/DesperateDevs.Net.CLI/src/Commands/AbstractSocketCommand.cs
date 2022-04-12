using System;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.CLI.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.Net.CLI {

    public abstract class AbstractSocketCommand : AbstractCommand {

        protected readonly Logger _logger;
        protected AbstractTcpSocket _socket;

        TcpMessageParser _tcpMessageParser;

        protected AbstractSocketCommand(string loggerName) {
            _logger = Sherlog.GetLogger(loggerName);
        }

        protected void start() {
            _tcpMessageParser = new TcpMessageParser();
            _tcpMessageParser.OnMessage += onMessage;
            _socket.OnReceived += onReceive;
            Console.CancelKeyPress += onCancel;
            while (true) {
                _socket.Send(TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(Console.ReadLine())));
            }
        }

        void onMessage(TcpMessageParser messageparser, byte[] bytes) {
            _logger.Info(Encoding.UTF8.GetString(bytes));
        }

        protected void onReceive(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            _tcpMessageParser.Receive(bytes);
        }

        protected void onCancel(object sender, ConsoleCancelEventArgs e) {
            _socket.Disconnect();
        }
    }
}
