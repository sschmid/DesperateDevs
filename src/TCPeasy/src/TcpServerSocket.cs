using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace TCPeasy
{
    public class TcpServerSocket : AbstractTcpSocket
    {
        public delegate void TcpServerSocketHandler(TcpServerSocket server, Socket client);

        public event TcpServerSocketHandler OnClientConnected;
        public event TcpServerSocketHandler OnClientDisconnected;

        public int Count => _clients.Count;

        readonly Dictionary<string, Socket> _clients = new Dictionary<string, Socket>();

        public TcpServerSocket() : base(typeof(TcpServerSocket).FullName) { }

        public void Listen(int port)
        {
            _logger.Info($"Listening on port {port}");
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(128);
            AcceptAsync();
        }

        void AcceptAsync()
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += OnAccept;
            if (!_socket.AcceptAsync(args))
                OnAccept(_socket, args);
        }

        void OnAccept(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                var key = KeyForEndPoint((IPEndPoint)args.AcceptSocket.RemoteEndPoint);
                _clients.Add(key, args.AcceptSocket);
                _logger.Debug($"Accepted new client connection from {key}");
                OnClientConnected?.Invoke(this, args.AcceptSocket);
                ReceiveAsync(args.AcceptSocket);
                AcceptAsync();
            }
        }

        protected override void OnReceive(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                var client = (Socket)sender;
                if (args.BytesTransferred == 0)
                {
                    if (client.Connected)
                    {
                        args.UserToken = sender;
                        OnClientDisconnect(sender, args);
                    }
                }
                else
                {
                    var key = KeyForEndPoint((IPEndPoint)client.RemoteEndPoint);
                    _logger.Debug($"Received {args.BytesTransferred} bytes from {key}");
                    TriggerOnReceived(client, args.Buffer, args.BytesTransferred);
                    ReceiveAsync(client);
                }
            }
        }

        public Socket GetClientWithRemoteEndPoint(IPEndPoint endPoint)
        {
            _clients.TryGetValue(KeyForEndPoint(endPoint), out var client);
            return client;
        }

        public override void Send(byte[] buffer)
        {
            if (_clients.Count != 0)
                foreach (var client in _clients.Values.ToArray())
                    Send(client, buffer);
            else
                _logger.Debug("No connected clients. Won't send.");
        }

        public void SendTo(byte[] buffer, IPEndPoint endPoint) =>
            Send(GetClientWithRemoteEndPoint(endPoint), buffer);

        public void DisconnectClient(IPEndPoint endPoint) =>
            DisconnectClient(GetClientWithRemoteEndPoint(endPoint));

        public override void Disconnect()
        {
            _socket.Close();
            _logger.Info("Stopped listening");
            foreach (var client in _clients.Values.ToArray())
                DisconnectClient(client);
        }

        void DisconnectClient(Socket client)
        {
            if (client != null && client.Connected)
            {
                client.Shutdown(SocketShutdown.Both);
                var args = new SocketAsyncEventArgs();
                args.Completed += OnClientDisconnect;
                if (!client.DisconnectAsync(args))
                    OnClientDisconnect(client, args);
            }
        }

        void OnClientDisconnect(object sender, SocketAsyncEventArgs args)
        {
            var client = (Socket)sender;
            var key = _clients.Single(kvp => kvp.Value == client).Key;
            _clients.Remove(key);
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            _logger.Debug(args.UserToken != null
                ? $"Client {key} disconnected"
                : $"Disconnected client {key}");
            OnClientDisconnected?.Invoke(this, client);
        }
    }
}
