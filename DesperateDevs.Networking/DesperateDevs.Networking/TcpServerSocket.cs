using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DesperateDevs.Networking {

    public class TcpServerSocket : AbstractTcpSocket {

        public delegate void TcpServerSocketHandler(TcpServerSocket server, Socket client);

        public event TcpServerSocketHandler OnClientConnected;
        public event TcpServerSocketHandler OnClientDisconnected;

        public int count { get { return _clients.Count; } }

        readonly Dictionary<string, Socket> _clients;

        public TcpServerSocket() : base(typeof(TcpServerSocket).Name) {
            _clients = new Dictionary<string, Socket>();
        }

        public void Listen(int port) {
            _logger.Info("Server is listening on port " + port + "...");
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(128);
            _socket.BeginAccept(onAccepted, _socket);
        }

        public Socket GetClientWithRemoteEndPoint(IPEndPoint endPoint) {
            Socket client;
            _clients.TryGetValue(keyForEndPoint(endPoint), out client);
            return client;
        }

        public override void Send(byte[] buffer) {
            if (_clients.Count != 0) {
                foreach (var client in _clients.Values.ToArray()) {
                    send(client, buffer);
                }
            } else {
                _logger.Info("Server doesn't have any connected clients. Won't send.");
            }
        }

        public void SendTo(byte[] buffer, IPEndPoint endPoint) {
            var client = GetClientWithRemoteEndPoint(endPoint);
            send(client, buffer);
        }

        public void DisconnectClient(IPEndPoint endPoint) {
            var client = GetClientWithRemoteEndPoint(endPoint);
            client.Shutdown(SocketShutdown.Both);
            client.BeginDisconnect(false, onDisconnectClient, client);
        }

        public override void Disconnect() {
            _socket.Close();
            _logger.Info("Server stopped listening");

            foreach (var client in _clients.Values.ToArray()) {
                client.Shutdown(SocketShutdown.Both);
                client.BeginDisconnect(false, onDisconnectClient, client);
            }
        }

        void onAccepted(IAsyncResult ar) {
            var server = (Socket)ar.AsyncState;
            Socket client = null;

            try {
                client = server.EndAccept(ar);
            } catch (ObjectDisposedException) {
                // Intended catch:
                // ObjectDisposedException "Cannot access a disposed object."

                // Caused by:
                // tcpServerSocket.Disconnect() calls socket.Close() while socket.BeginAccept() is in progress.

                // Explanation:
                // When the socket.Close() method is called while an asynchronous operation is in progress,
                // the callback provided to the socket.BeginAccept method is called.
                // A subsequent call to the socket.EndAccept method will throw an ObjectDisposedException
                // to indicate that the operation has been cancelled.
            }

            if (client != null) {
                addClient(client);
                _socket.BeginAccept(onAccepted, _socket);
            }
        }

        void addClient(Socket client) {
            var key = keyForEndPoint((IPEndPoint)client.RemoteEndPoint);
            _clients.Add(key, client);

            _logger.Info("Server accepted new client connection from " + key);

            receive(new ReceiveVO(client, new byte[client.ReceiveBufferSize]));

            if (OnClientConnected != null) {
                OnClientConnected(this, client);
            }
        }

        protected override void onReceived(IAsyncResult ar) {
            var receiveVO = (ReceiveVO)ar.AsyncState;
            var bytesReceived = 0;
            try {
                bytesReceived = receiveVO.socket.EndReceive(ar);
            } catch (SocketException) {
                // Intended catch:
                // SocketException "interrupted"

                // Caused by:
                // tcpServerSocket.Disconnect() disconnects each client while socket.BeginReceive() is in progress.
                // This will interrupt
            }

            if (bytesReceived == 0) {
                if (receiveVO.socket.Connected) {
                    removeClient(receiveVO.socket);
                } else {
                    // Server manually disconnected client via server.Disconnect() and will
                    // close client in onDisconnectClient()
                }
            } else {
                var key = keyForEndPoint((IPEndPoint)receiveVO.socket.RemoteEndPoint);
                _logger.Debug("Server received " + bytesReceived + " bytes from " + key);

                triggerOnReceived(receiveVO, bytesReceived);

                receive(receiveVO);
            }
        }

        void removeClient(Socket socket) {
            var key = _clients.Single(kv => kv.Value == socket).Key;
            _clients.Remove(key);
            socket.Close();
            _logger.Info("Client " + key + " disconnected from server");

            if (OnClientDisconnected != null) {
                OnClientDisconnected(this, socket);
            }
        }

        void onDisconnectClient(IAsyncResult ar) {
            var client = (Socket)ar.AsyncState;
            var key = _clients.Single(kv => kv.Value == client).Key;
            _clients.Remove(key);
            client.EndDisconnect(ar);
            client.Close();
            _logger.Info("Server disconnected client " + key);
        }
    }
}
