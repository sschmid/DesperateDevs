using System;
using System.Net;
using System.Net.Sockets;
using DesperateDevs.Logging;

namespace DesperateDevs.Net
{
    public class ReceiveVO
    {
        public readonly Socket socket;
        public readonly byte[] bytes;

        public ReceiveVO(Socket socket, byte[] bytes)
        {
            this.socket = socket;
            this.bytes = bytes;
        }
    }

    public delegate void TcpSocketReceive(AbstractTcpSocket tcpSocket, Socket socket, byte[] bytes);

    public abstract class AbstractTcpSocket
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

        protected void send(Socket socket, byte[] buffer)
        {
            var key = keyForEndPoint((IPEndPoint)socket.RemoteEndPoint);
            _logger.Debug("Sending " + buffer.Length + " bytes via " + key);

            socket.BeginSend(
                buffer,
                0,
                buffer.Length,
                SocketFlags.None,
                onSent,
                socket
            );
        }

        void onSent(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            {
                // ignored
            }
        }

        protected void receive(ReceiveVO receiveVO)
        {
            receiveVO.socket.BeginReceive(
                receiveVO.bytes,
                0,
                receiveVO.bytes.Length,
                SocketFlags.None,
                onReceived,
                receiveVO
            );
        }

        protected abstract void onReceived(IAsyncResult ar);

        public abstract void Disconnect();

        protected void triggerOnReceived(ReceiveVO receiveVO, int bytesReceived)
        {
            if (OnReceived != null)
            {
                OnReceived(this, receiveVO.socket, trimmedBytes(receiveVO.bytes, bytesReceived));
            }
        }

        protected static string keyForEndPoint(IPEndPoint endPoint)
        {
            return endPoint.Address + ":" + endPoint.Port;
        }

        static byte[] trimmedBytes(byte[] bytes, int length)
        {
            var trimmed = new byte[length];
            Array.Copy(bytes, trimmed, length);
            return trimmed;
        }
    }
}
