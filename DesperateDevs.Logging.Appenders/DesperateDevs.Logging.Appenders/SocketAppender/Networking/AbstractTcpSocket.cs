using System;
using System.Net.Sockets;

namespace DesperateDevs.Logging.Appenders {

    public struct ReceiveVO {
        public Socket socket;
        public byte[] buffer;
    }

    public abstract class AbstractTcpSocket {

        public delegate void TcpSocketReceive(AbstractTcpSocket socket, Socket client, byte[] bytes);
        public delegate void TcpSocketDisconnect(AbstractTcpSocket socket);

        public event TcpSocketReceive OnReceive;
        public event TcpSocketDisconnect OnDisconnect;

        public bool isConnected { get; protected set; }

        protected Logger _logger;
        protected Socket _socket;

        protected void triggerOnReceive(ReceiveVO receiveVO, int bytesReceived) {
            if (OnReceive != null) {
                OnReceive(this, receiveVO.socket, trimmedBuffer(receiveVO.buffer, bytesReceived));
            }
        }

        protected void triggerOnDisconnect() {
            if (OnDisconnect != null) {
                OnDisconnect(this);
            }
        }

        protected void startReceiving(Socket socket) {
            var receiveVO = new ReceiveVO {
                socket = socket,
                buffer = new byte[socket.ReceiveBufferSize]
            };
            receive(receiveVO);
        }

        protected void receive(ReceiveVO receiveVO) {
            receiveVO.socket.BeginReceive(receiveVO.buffer, 0,
                receiveVO.buffer.Length, SocketFlags.None, onReceived, receiveVO);
        }

        protected void onReceived(IAsyncResult ar) {
            var receiveVO = (ReceiveVO)ar.AsyncState;
            if (isConnected) {
                var bytesReceived = receiveVO.socket.EndReceive(ar);

                if (bytesReceived == 0) {
                    disconnectedByRemote(receiveVO.socket);
                } else {
                    _logger.Debug("Received " + bytesReceived + " bytes.");
                    triggerOnReceive(receiveVO, bytesReceived);

                    receive(receiveVO);
                }
            }
        }

        public void SendWith(Socket socket, byte[] bytes) {
            if (isConnected && socket.Connected) {
                socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, onSent, socket);
            }
        }

        void onSent(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        protected abstract void disconnectedByRemote(Socket socket);

        protected byte[] trimmedBuffer(byte[] buffer, int length) {
            var trimmed = new byte[length];
            Array.Copy(buffer, trimmed, length);
            return trimmed;
        }

        public abstract void Send(byte[] bytes);

        public abstract void Disconnect();
    }
}
