using System.Collections.Generic;
using System.Net;
using DesperateDevs.Networking;

namespace DesperateDevs.Logging.Appenders {

    public abstract class AbstractTcpSocketAppender {

        public AbstractTcpSocket socket { get; private set; }

        static readonly Logger _logger = fabl.GetLogger(typeof(AbstractTcpSocketAppender).Name);

        readonly List<HistoryItem> _history = new List<HistoryItem>();

        public void Connect(IPAddress ip, int port) {
            var client = new TcpClientSocket();
            socket = client;
            client.OnConnected += sender => onConnected();
            client.Connect(ip, port);
        }

        public void Listen(int port) {
            var server = new TcpServerSocket();
            socket = server;
            server.OnClientConnected += (sender, client) => onConnected();
            server.Listen(port);
        }

        public void Disconnect() {
            socket.Disconnect();
        }

        public void Send(Logger logger, LogLevel logLevel, string message) {
            if (isSocketReady()) {
                socket.Send(serializeMessage(logger, logLevel, message));
            } else {
                _history.Add(new HistoryItem(logger, logLevel, message));
            }
        }

        bool isSocketReady() {
            if (socket != null) {
                var server = socket as TcpServerSocket;
                if (server != null) {
                    return server.count > 0;
                }

                var client = socket as TcpClientSocket;
                if (client != null) {
                    return client.isConnected;
                }
            }

            return false;
        }

        void onConnected() {
            if (_history.Count > 0) {
                Send(_logger, LogLevel.Debug, "Flush history - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                foreach (var item in _history) {
                    Send(item.logger, item.logLevel, item.message);
                }
                Send(_logger, LogLevel.Debug, "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                _history.Clear();
            }
        }

        protected abstract byte[] serializeMessage(Logger logger, LogLevel logLevel, string message);

        class HistoryItem {

            public readonly Logger logger;
            public readonly LogLevel logLevel;
            public readonly string message;

            public HistoryItem(Logger logger, LogLevel logLevel, string message) {
                this.logger = logger;
                this.logLevel = logLevel;
                this.message = message;
            }
        }
    }
}
