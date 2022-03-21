using System.Collections.Generic;
using System.Net;
using DesperateDevs.Networking;

namespace DesperateDevs.Logging.Appenders
{
    public abstract class AbstractTcpSocketAppender
    {
        public AbstractTcpSocket Socket { get; private set; }

        readonly Logger _logger = Sherlog.GetLogger(typeof(AbstractTcpSocketAppender));

        readonly List<HistoryEntry> _history = new List<HistoryEntry>();

        public void Connect(IPAddress ip, int port)
        {
            var client = new TcpClientSocket();
            Socket = client;
            client.OnConnected += sender => OnConnected();
            client.Connect(ip, port);
        }

        public void Listen(int port)
        {
            var server = new TcpServerSocket();
            Socket = server;
            server.OnClientConnected += (sender, client) => OnConnected();
            server.Listen(port);
        }

        public void Disconnect() => Socket.Disconnect();

        public void Send(Logger logger, LogLevel logLevel, string message)
        {
            if (IsSocketReady())
                Socket.Send(SerializeMessage(logger, logLevel, message));
            else
                _history.Add(new HistoryEntry(logger, logLevel, message));
        }

        bool IsSocketReady()
        {
            if (Socket != null)
            {
                if (Socket is TcpServerSocket server)
                    return server.count > 0;

                if (Socket is TcpClientSocket client)
                    return client.isConnected;
            }

            return false;
        }

        void OnConnected()
        {
            if (_history.Count > 0)
            {
                foreach (var entry in _history)
                    Send(entry.Logger, entry.LogLevel, entry.Message);

                _history.Clear();
            }
        }

        protected abstract byte[] SerializeMessage(Logger logger, LogLevel logLevel, string message);

        class HistoryEntry
        {
            public readonly Logger Logger;
            public readonly LogLevel LogLevel;
            public readonly string Message;

            public HistoryEntry(Logger logger, LogLevel logLevel, string message)
            {
                Logger = logger;
                LogLevel = logLevel;
                Message = message;
            }
        }
    }
}
