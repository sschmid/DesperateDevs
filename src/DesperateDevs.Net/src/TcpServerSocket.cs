﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DesperateDevs.Net
{
    public class TcpServerSocket : AbstractTcpSocket
    {
        public delegate void TcpServerSocketHandler(TcpServerSocket server, Socket client);

        public event TcpServerSocketHandler OnClientConnected;
        public event TcpServerSocketHandler OnClientDisconnected;

        public int Count => _clients.Count;

        readonly Dictionary<string, Socket> _clients;

        public TcpServerSocket() : base(typeof(TcpServerSocket).FullName)
        {
            _clients = new Dictionary<string, Socket>();
        }

        public void Listen(int port)
        {
            _logger.Info("Server is listening on port " + port + "...");
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(128);
            _socket.BeginAccept(OnAccept, _socket);
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
                _logger.Debug("Server doesn't have any connected clients. Won't send.");
        }

        public void SendTo(byte[] buffer, IPEndPoint endPoint) =>
            Send(GetClientWithRemoteEndPoint(endPoint), buffer);

        public void DisconnectClient(IPEndPoint endPoint)
        {
            var client = GetClientWithRemoteEndPoint(endPoint);
            client.Shutdown(SocketShutdown.Both);
            client.BeginDisconnect(false, OnDisconnectClient, client);
        }

        public override void Disconnect()
        {
            _socket.Close();
            _logger.Info("Server stopped listening");

            foreach (var client in _clients.Values.ToArray())
            {
                client.Shutdown(SocketShutdown.Both);
                client.BeginDisconnect(false, OnDisconnectClient, client);
            }
        }

        void OnAccept(IAsyncResult ar)
        {
            var server = (Socket)ar.AsyncState;
            Socket client = null;

            try
            {
                client = server.EndAccept(ar);
            }
            catch (ObjectDisposedException)
            {
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

            if (client != null)
            {
                AddClient(client);
                _socket.BeginAccept(OnAccept, _socket);
            }
        }

        void AddClient(Socket client)
        {
            var key = KeyForEndPoint((IPEndPoint)client.RemoteEndPoint);
            _clients.Add(key, client);
            _logger.Debug("Server accepted new client connection from " + key);
            Receive(new ReceiveVO(client, new byte[client.ReceiveBufferSize]));
            OnClientConnected?.Invoke(this, client);
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
                // tcpServerSocket.Disconnect() disconnects each client while socket.BeginReceive() is in progress.
                // This will interrupt
            }

            if (bytesReceived == 0)
            {
                if (receiveVO.Socket.Connected)
                {
                    RemoveClient(receiveVO.Socket);
                }
                else
                {
                    // Server manually disconnected client via server.Disconnect() and will
                    // close client in onDisconnectClient()
                }
            }
            else
            {
                var key = KeyForEndPoint((IPEndPoint)receiveVO.Socket.RemoteEndPoint);
                _logger.Debug("Server received " + bytesReceived + " bytes from " + key);
                TriggerOnReceived(receiveVO, bytesReceived);
                Receive(receiveVO);
            }
        }

        void RemoveClient(Socket socket)
        {
            var key = _clients.Single(kv => kv.Value == socket).Key;
            _clients.Remove(key);
            socket.Close();
            _logger.Debug("Client " + key + " disconnected from server");
            OnClientDisconnected?.Invoke(this, socket);
        }

        void OnDisconnectClient(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;
            var key = _clients.Single(kv => kv.Value == client).Key;
            _clients.Remove(key);
            client.EndDisconnect(ar);
            client.Close();
            _logger.Debug("Server disconnected client " + key);
        }
    }
}
