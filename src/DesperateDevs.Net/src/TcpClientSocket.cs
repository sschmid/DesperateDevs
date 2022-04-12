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

        public bool isConnected
        {
            get { return _socket.Connected; }
        }

        public TcpClientSocket() : base(typeof(TcpClientSocket).FullName) { }

        public void Connect(IPAddress ipAddress, int port)
        {
            _logger.Debug("Client is connecting to " + ipAddress + ":" + port + "...");
            _socket.BeginConnect(ipAddress, port, onConnected, _socket);
        }

        public override void Send(byte[] buffer)
        {
            send(_socket, buffer);
        }

        public override void Disconnect()
        {
            _logger.Debug("Client is disconnecting...");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.BeginDisconnect(false, onDisconnected, _socket);
        }

        void onConnected(IAsyncResult ar)
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
                // SocketException "Conn    ection refused"

                // Caused by
                // Port is not beeing listened to.

                _logger.Error(ex.Message);
            }

            if (didConnect)
            {
                var rep = (IPEndPoint)client.RemoteEndPoint;
                _logger.Debug("Client connected to " + keyForEndPoint(rep));

                receive(new ReceiveVO(client, new byte[client.ReceiveBufferSize]));

                if (OnConnected != null)
                {
                    OnConnected(this);
                }
            }
        }

        protected override void onReceived(IAsyncResult ar)
        {
            var receiveVO = (ReceiveVO)ar.AsyncState;
            var bytesReceived = 0;
            try
            {
                bytesReceived = receiveVO.socket.EndReceive(ar);
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
                if (receiveVO.socket.Connected)
                {
                    disconnectedByRemote(receiveVO.socket);
                }
                else
                {
                    // Client manually disconnected via client.Disconnect() and will
                    // be closed in onDisconnected()
                }
            }
            else
            {
                var key = keyForEndPoint((IPEndPoint)receiveVO.socket.RemoteEndPoint);
                _logger.Debug("Client received " + bytesReceived + " bytes from " + key);

                triggerOnReceived(receiveVO, bytesReceived);

                receive(receiveVO);
            }
        }

        void disconnectedByRemote(Socket client)
        {
            client.Close();
            _logger.Info("Client got disconnected by remote");
            if (OnDisconnected != null)
            {
                OnDisconnected(this);
            }
        }

        void onDisconnected(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
            client.Close();
            _logger.Debug("Client disconnected");

            if (OnDisconnected != null)
            {
                OnDisconnected(this);
            }
        }
    }
}
