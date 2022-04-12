using System;
using System.Net;
using System.Net.Sockets;
using DesperateDevs.Logging;

namespace DesperateDevs.Net
{
    public class ReceiveVO
    {
        public readonly Socket Socket;
        public readonly byte[] Bytes;

        public ReceiveVO(Socket socket, byte[] bytes)
        {
            Socket = socket;
            Bytes = bytes;
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

        protected void Send(Socket socket, byte[] buffer)
        {
            var key = KeyForEndPoint((IPEndPoint)socket.RemoteEndPoint);
            _logger.Debug("Sending " + buffer.Length + " bytes via " + key);

            socket.BeginSend(
                buffer,
                0,
                buffer.Length,
                SocketFlags.None,
                OnSent,
                socket
            );
        }

        void OnSent(IAsyncResult ar)
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

        protected void Receive(ReceiveVO receiveVO)
        {
            receiveVO.Socket.BeginReceive(
                receiveVO.Bytes,
                0,
                receiveVO.Bytes.Length,
                SocketFlags.None,
                OnReceive,
                receiveVO
            );
        }

        protected abstract void OnReceive(IAsyncResult ar);

        public abstract void Disconnect();

        protected void TriggerOnReceived(ReceiveVO receiveVO, int bytesReceived)
        {
            if (OnReceived != null)
            {
                OnReceived(this, receiveVO.Socket, TrimmedBytes(receiveVO.Bytes, bytesReceived));
            }
        }

        protected static string KeyForEndPoint(IPEndPoint endPoint)
        {
            return endPoint.Address + ":" + endPoint.Port;
        }

        static byte[] TrimmedBytes(byte[] bytes, int length)
        {
            var trimmed = new byte[length];
            Array.Copy(bytes, trimmed, length);
            return trimmed;
        }
    }
}
