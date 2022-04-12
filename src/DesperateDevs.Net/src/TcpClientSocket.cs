using System;
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

        public void Connect(IPAddress ipAddress, int port)
        {
            _logger.Debug("Client is connecting to " + ipAddress + ":" + port + "...");
            _socket.BeginConnect(ipAddress, port, OnConnect, _socket);
        }

        public override void Send(byte[] buffer) => Send(_socket, buffer);

        public override void Disconnect()
        {
            _logger.Debug("Client is disconnecting...");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.BeginDisconnect(false, OnDisconnect, _socket);
        }

        void OnConnect(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;

            var didConnect = false;
            try
            {
                client.EndConnect(ar);
                didConnect = true;
            }
            catch (SocketException ex)
            {
                // Intended catch:
                // SocketException "Connection refused"

                // Caused by
                // Port is not being listened to.

                _logger.Error(ex.Message);
            }

            if (didConnect)
            {
                var rep = (IPEndPoint)client.RemoteEndPoint;
                _logger.Debug("Client connected to " + KeyForEndPoint(rep));
                Receive(new ReceiveVO(client, new byte[client.ReceiveBufferSize]));
                OnConnected?.Invoke(this);
            }
        }

        protected override void OnReceive(IAsyncResult ar)
        {
            var receiveVO = (ReceiveVO)ar.AsyncState;
            var bytesReceived = 0;
            try
            {
                bytesReceived = receiveVO.Socket.EndReceive(ar);
            }
            catch (SocketException)
            {
                // Intended catch:
                // SocketException "interrupted"

                // Caused by:
                // tcpClientSocket.Disconnect() disconnects client while socket.BeginReceive() is in progress.
                // This will interrupt
            }

            if (bytesReceived == 0)
            {
                if (receiveVO.Socket.Connected)
                {
                    DisconnectedByRemote(receiveVO.Socket);
                }
                else
                {
                    // Client manually disconnected via client.Disconnect() and will
                    // be closed in OnDisconnect()
                }
            }
            else
            {
                var key = KeyForEndPoint((IPEndPoint)receiveVO.Socket.RemoteEndPoint);
                _logger.Debug("Client received " + bytesReceived + " bytes from " + key);
                TriggerOnReceived(receiveVO, bytesReceived);
                Receive(receiveVO);
            }
        }

        void DisconnectedByRemote(Socket client)
        {
            client.Close();
            _logger.Info("Client got disconnected by remote");
            OnDisconnected?.Invoke(this);
        }

        void OnDisconnect(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
            client.Close();
            _logger.Debug("Client disconnected");
            OnDisconnected?.Invoke(this);
        }
    }
}
