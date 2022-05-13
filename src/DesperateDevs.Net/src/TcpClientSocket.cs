using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DesperateDevs.Net
{
    public class TcpClientSocket : AbstractTcpSocket
    {
        public delegate void TcpClientSocketHandler(TcpClientSocket client);

        public event TcpClientSocketHandler OnConnected;
        public event TcpClientSocketHandler OnDisconnected;

        public bool IsConnected => _socket.Connected;

        public TcpClientSocket() : base(typeof(TcpClientSocket).FullName) { }

        public void Connect(string host, int port)
        {
            var ipAddress = Dns.GetHostEntry(host).AddressList
                .FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);

            Connect(ipAddress, port);
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            _logger.Debug($"Connecting to {ipAddress}:{port}...");
            var args = new SocketAsyncEventArgs
            {
                RemoteEndPoint = new IPEndPoint(ipAddress, port)
            };
            args.Completed += OnConnect;
            if (!_socket.ConnectAsync(args))
                OnConnect(_socket, args);
        }

        void OnConnect(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                var client = (Socket)sender;
                var key = KeyForEndPoint((IPEndPoint)client.RemoteEndPoint);
                _logger.Debug($"Connected to {key}");
                OnConnected?.Invoke(this);
                ReceiveAsync(client);
            }
            else
            {
                _logger.Error(args.SocketError.ToString());
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
                        DisconnectedByRemote(client);
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

        void DisconnectedByRemote(Socket client)
        {
            _logger.Info("Disconnected by remote");
            Disconnect();
        }

        public override void Disconnect()
        {
            _logger.Debug("Disconnecting...");
            _socket.Shutdown(SocketShutdown.Both);
            var args = new SocketAsyncEventArgs
            {
                DisconnectReuseSocket = true
            };
            args.Completed += OnDisconnect;
            if (!_socket.DisconnectAsync(args))
                OnDisconnect(_socket, args);
        }

        void OnDisconnect(object sender, SocketAsyncEventArgs args)
        {
            _logger.Debug("Disconnected");
            OnDisconnected?.Invoke(this);
        }

        public override void Send(byte[] buffer) => Send(_socket, buffer);
    }
}
