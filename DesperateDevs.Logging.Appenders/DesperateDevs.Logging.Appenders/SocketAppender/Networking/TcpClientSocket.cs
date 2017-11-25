using System;
using System.Net;
using System.Net.Sockets;

namespace DesperateDevs.Logging.Appenders {

    public class TcpClientSocket : AbstractTcpSocket {

        public delegate void TcpClientSocketConnect(TcpClientSocket client);

        public event TcpClientSocketConnect OnConnect;

        public TcpClientSocket() {
            _logger = fabl.GetLogger(GetType().Name);
        }

        public void Connect(IPAddress ip, int port) {
            _logger.Debug("Connecting to " + ip + ":" + port + "...");
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(ip, port, onConnected, _socket);
        }

        void onConnected(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            try {
                socket.EndConnect(ar);
                isConnected = true;
                var clientEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                _logger.Info("Connected to " + clientEndPoint.Address + ":" + clientEndPoint.Port);
                if (OnConnect != null) {
                    OnConnect(this);
                }
                startReceiving(socket);
            } catch (Exception ex) {
                _logger.Warn(ex.Message);
                triggerOnDisconnect();
            }
        }

        public override void Send(byte[] bytes) {
            SendWith(_socket, bytes);
        }

        protected override void disconnectedByRemote(Socket socket) {
            _logger.Info("Disconnected by remote.");
            Disconnect();
        }

        public override void Disconnect() {
            if (isConnected) {
                _logger.Debug("Disconnecting...");
                isConnected = false;
                _socket.BeginDisconnect(false, onDisconnected, _socket);
            } else {
                _logger.Debug("Already diconnected.");
            }
        }

        void onDisconnected(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            socket.EndDisconnect(ar);
            socket.Close();
            _socket = null;
            _logger.Debug("Disconnected.");
            triggerOnDisconnect();
        }
    }
}
