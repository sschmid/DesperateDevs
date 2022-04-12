using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Net.Tests
{
    [Collection("Non-Parallel")]
    public class TcpServerSocketTests : IDisposable
    {
        const int Port = 1234;
        readonly IPAddress _ipAddress = IPAddress.Loopback;
        readonly TcpServerSocket _server = new TcpServerSocket();

        Socket _client1;
        Socket _client2;
        IPEndPoint _rep1;
        IPEndPoint _rep2;

        [Fact]
        public void InvokingDisconnectOnDisconnectedServerDoesNothing()
        {
            _server.Disconnect();
            _server.Disconnect();
        }

        [Fact]
        public void NewServerSocketDoesNotHaveConnectedClients()
        {
            Listen();
            _server.count.Should().Be(0);
        }

        [Fact]
        public void CanSendWithoutHavingConnectedClients()
        {
            Listen();
            _server.Send(Encoding.UTF8.GetBytes("test"));
            Wait();
        }

        [Fact]
        public void AcceptsClientConnection()
        {
            Listen();
            ConnectClient(out _client1);
            _server.count.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnClientConnected()
        {
            var connected = 0;
            TcpServerSocket eventServer = null;
            _server.OnClientConnected += (s, c) =>
            {
                connected += 1;
                eventServer = s;
            };

            Listen();
            ConnectClient(out _client1);
            connected.Should().Be(1);
            eventServer.Should().BeSameAs(_server);
        }

        [Fact]
        public void GetsClientByEndPoint()
        {
            Socket eventClient = null;
            _server.OnClientConnected += (s, c) => { eventClient = c; };

            Listen();
            ConnectClient(out _client1);

            var rep = (IPEndPoint)eventClient.RemoteEndPoint;
            var key = new IPEndPoint(IPAddress.Parse(rep.Address.ToString()), rep.Port);

            var client = _server.GetClientWithRemoteEndPoint(key);
            client.Should().BeSameAs(eventClient);
        }

        [Fact]
        public void RemovesDisconnectedClient()
        {
            Listen();
            ConnectClientWithEndPoint1(out _client1);
            DisconnectClient(ref _client1);
            _server.count.Should().Be(0);
        }

        [Fact]
        public void DispatchesOnClientDisconnected()
        {
            var disconnected = 0;
            TcpServerSocket eventServer = null;

            _server.OnClientDisconnected += (s, c) =>
            {
                disconnected += 1;
                eventServer = s;
            };

            Listen();
            ConnectClient(out _client1);
            DisconnectClient(ref _client1);

            disconnected.Should().Be(1);
            eventServer.Should().BeSameAs(_server);
        }

        [Fact]
        public void ServerDisconnectsAndRemovesClients()
        {
            Listen();
            ConnectClient(out _client1);
            _server.Disconnect();
            Wait();
            _server.count.Should().Be(0);
        }

        [Fact]
        public void DisconnectsClient()
        {
            Listen();
            ConnectClientWithEndPoint1(out _client1);
            PrepareForReceive(_client1, s => { });

            var client = _server.GetClientWithRemoteEndPoint(_rep1);
            client.Connected.Should().BeTrue();

            _server.DisconnectClient(_rep1);
            Wait();

            client.Connected.Should().BeFalse();
        }

        [Fact]
        public void ReceivesClientMessage()
        {
            AbstractTcpSocket eventServer = null;
            byte[] eventBytes = null;
            _server.OnReceived += (tcpSocket, socket, bytes) =>
            {
                eventServer = tcpSocket;
                eventBytes = bytes;
            };

            Listen();
            ConnectClient(out _client1);
            var buffer = Encoding.UTF8.GetBytes("test");
            _client1.Send(buffer, 0, buffer.Length, SocketFlags.None);
            Wait();

            eventServer.Should().BeSameAs(_server);
            eventBytes.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void SendsMessageToClient()
        {
            string eventMsg = null;
            Listen();
            ConnectClient(out _client1);
            PrepareForReceive(_client1, s => eventMsg = s);

            const string message = "test";
            var buffer = Encoding.UTF8.GetBytes(message);
            _server.Send(buffer);
            Wait();

            eventMsg.Should().Be(message);
        }

        [Fact]
        public void ReceivesMultipleClientMessages()
        {
            var eventBytes = new List<byte[]>();
            _server.OnReceived += (sender, receiver, bytes) => { eventBytes.Add(bytes); };

            Listen();
            ConnectClient(out _client1);

            var buffer1 = Encoding.UTF8.GetBytes("test1");
            var buffer2 = Encoding.UTF8.GetBytes("test2");
            _client1.Send(buffer1, 0, buffer1.Length, SocketFlags.None);
            Wait();
            _client1.Send(buffer2, 0, buffer2.Length, SocketFlags.None);
            Wait();

            eventBytes[0].Should().BeEquivalentTo(buffer1);
            eventBytes[1].Should().BeEquivalentTo(buffer2);
        }

        [Fact]
        public void AcceptsMultipleClientConnection()
        {
            Listen();
            ConnectClient(out _client1);
            ConnectClient(out _client2);
            _server.count.Should().Be(2);
        }

        [Fact]
        public void SendsMessageToAllConnectedClients()
        {
            Listen();
            ConnectClient(out _client1);
            ConnectClient(out _client2);
            string eventMsg1 = null;
            string eventMsg2 = null;
            PrepareForReceive(_client1, s => eventMsg1 = s);
            PrepareForReceive(_client2, s => eventMsg2 = s);

            const string message = "test";
            var buffer = Encoding.UTF8.GetBytes(message);
            _server.Send(buffer);
            Wait();

            eventMsg1.Should().Be(message);
            eventMsg2.Should().Be(message);
        }

        [Fact]
        public void SendsMessageToSpecifiedClient()
        {
            Listen();
            ConnectClient(out _client1);
            ConnectClientWithEndPoint2(out _client2);
            string eventMsg1 = null;
            string eventMsg2 = null;
            PrepareForReceive(_client1, s => eventMsg1 = s, true);
            PrepareForReceive(_client2, s => eventMsg2 = s);

            const string message = "test";
            var buffer = Encoding.UTF8.GetBytes(message);
            _server.SendTo(buffer, _rep2);
            Wait();

            eventMsg1.Should().BeNull();
            eventMsg2.Should().Be(message);
        }

        Socket CreateClient() => new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        void Listen()
        {
            _server.Listen(Port);
            Wait();
        }

        void ConnectClient(out Socket client)
        {
            client = CreateClient();
            client.Connect(_ipAddress, Port);
            Wait();
        }

        void ConnectClientWithEndPoint1(out Socket client)
        {
            _server.OnClientConnected += (socket, c) => { _rep1 = (IPEndPoint)c.RemoteEndPoint; };
            ConnectClient(out client);
        }

        void ConnectClientWithEndPoint2(out Socket client)
        {
            _server.OnClientConnected += (socket, c) => { _rep2 = (IPEndPoint)c.RemoteEndPoint; };
            ConnectClient(out client);
        }

        void DisconnectClient(ref Socket client)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Disconnect(false);
            client.Close();
            Wait();
            client = null;
        }

        void PrepareForReceive(Socket socket, Action<string> onReceive, bool mayFail = false, Action failAction = null)
        {
            var buffer = new byte[socket.ReceiveBufferSize];
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                ar =>
                {
                    var client = (Socket)ar.AsyncState;

                    if (mayFail)
                    {
                        try
                        {
                            var bytesReceived = client.EndReceive(ar);
                            var trimmedBuffer = new byte[bytesReceived];
                            Array.Copy(buffer, trimmedBuffer, bytesReceived);
                            onReceive(Encoding.UTF8.GetString(trimmedBuffer));
                        }
                        catch (Exception)
                        {
                            if (failAction != null)
                            {
                                failAction();
                            }
                        }
                    }
                    else
                    {
                        var bytesReceived = client.EndReceive(ar);
                        var trimmedBuffer = new byte[bytesReceived];
                        Array.Copy(buffer, trimmedBuffer, bytesReceived);
                        onReceive(Encoding.UTF8.GetString(trimmedBuffer));
                    }
                }, socket);

            Wait();
        }

        public void Dispose()
        {
            if (_client1 != null) DisconnectClient(ref _client1);
            if (_client2 != null) DisconnectClient(ref _client2);
            _server.Disconnect();
            Wait();
        }

        static void Wait() => System.Threading.Thread.Sleep(50);
    }
}
