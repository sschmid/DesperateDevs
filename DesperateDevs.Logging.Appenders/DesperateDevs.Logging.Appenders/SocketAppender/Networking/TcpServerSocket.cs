using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DesperateDevs.Logging.Appenders {

    public class TcpServerSocket : AbstractTcpSocket {

        public delegate void TcpSocketHandler(TcpServerSocket server, Socket client);

        public event TcpSocketHandler OnClientConnect;
        public event TcpSocketHandler OnClientDisconnect;

        public int connectedClients { get { return _clients.Count; } }

        readonly List<Socket> _clients;

        public TcpServerSocket() {
            _logger = fabl.GetLogger(GetType().Name);
            _clients = new List<Socket>();
        }

        public void Listen(int port) {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(100);
                isConnected = true;
                _logger.Info(string.Format("Listening on port " + port + "..."));
                accept();
            } catch (Exception ex) {
                _socket = null;
                _logger.Warn(ex.Message);
            }
        }

        void accept() {
            _socket.BeginAccept(onAccepted, _socket);
        }

        void onAccepted(IAsyncResult ar) {
            if (isConnected) {
                var server = (Socket)ar.AsyncState;
                acceptedClientConnection(server.EndAccept(ar));
                accept();
            }
        }

        void acceptedClientConnection(Socket client) {
            _clients.Add(client);
            var clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
            _logger.Info("New client connection accepted (" + clientEndPoint.Address +":" + clientEndPoint.Port + ")");
            if (OnClientConnect != null) {
                OnClientConnect(this, client);
            }

            startReceiving(client);
        }

        protected override void disconnectedByRemote(Socket socket) {
            try {
                var clientEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                _logger.Info("Client disconnected (" + clientEndPoint.Address + ":" + clientEndPoint.Port + ")");
            } catch (Exception) {
                _logger.Info("Client disconnected.");
            }
            socket.Close();
            _clients.Remove(socket);
            if (OnClientDisconnect != null) {
                OnClientDisconnect(this, socket);
            }
        }

        public override void Send(byte[] bytes) {
            foreach (var client in _clients) {
                SendWith(client, bytes);
            }
        }

        public override void Disconnect() {
            foreach (var client in _clients) {
                client.BeginDisconnect(false, onClientDisconnected, client);
            }

            if (isConnected) {
                _logger.Info("Stopped listening.");
                isConnected = false;
                _socket.Close();
                triggerOnDisconnect();
            } else {
                _logger.Info("Already diconnected.");
            }
        }

        void onClientDisconnected(IAsyncResult ar) {
            var client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
            disconnectedByRemote(client);
        }
    }
}
