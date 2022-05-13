using System;
using System.Net;
using System.Net.Sockets;
using DesperateDevs.Logging;

namespace DesperateDevs.Net
{
    public delegate void TcpSocketReceive(AbstractTcpSocket tcpSocket, Socket socket, byte[] bytes);

    public abstract class AbstractTcpSocket : IDisposable
    {
        public event TcpSocketReceive OnReceived;

        protected readonly Logger _logger;
        protected readonly Socket _socket;

        protected AbstractTcpSocket(string loggerName)
        {
            _logger = Sherlog.GetLogger(loggerName);
            _socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
        }

        public abstract void Send(byte[] buffer);

        protected void Send(Socket socket, byte[] buffer)
        {
            var key = KeyForEndPoint((IPEndPoint)socket.RemoteEndPoint);
            _logger.Debug($"Sending {buffer.Length} bytes via {key}...");
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(buffer, 0, buffer.Length);
            args.Completed += OnSend;
            if (!socket.SendAsync(args))
                OnSend(socket, args);
        }

        void OnSend(object sender, SocketAsyncEventArgs args)
        {
            var key = KeyForEndPoint((IPEndPoint)((Socket)sender).RemoteEndPoint);
            _logger.Debug($"Sent {args.Buffer.Length} bytes via {key}...");
        }

        protected void ReceiveAsync(Socket socket)
        {
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(new byte[socket.ReceiveBufferSize], 0, socket.ReceiveBufferSize);
            args.Completed += OnReceive;
            if (!socket.ReceiveAsync(args))
                OnReceive(socket, args);
        }

        protected abstract void OnReceive(object sender, SocketAsyncEventArgs args);

        public abstract void Disconnect();

        protected void TriggerOnReceived(Socket socket, byte[] bytes, int bytesReceived) =>
            OnReceived?.Invoke(this, socket, TrimmedBytes(bytes, bytesReceived));

        protected static string KeyForEndPoint(IPEndPoint endPoint) => $"{endPoint.Address}:{endPoint.Port}";

        static byte[] TrimmedBytes(byte[] bytes, int length)
        {
            var trimmed = new byte[length];
            Array.Copy(bytes, trimmed, length);
            return trimmed;
        }

        public void Dispose()
        {
            _socket.Close();
            _socket.Dispose();
        }
    }
}
