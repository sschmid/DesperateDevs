using System;
using System.Net;
using System.Text;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Net.Tests
{
    [Collection("Non-Parallel")]
    public class TcpClientSocketTests : IDisposable
    {
        const int Port = 4321;

        readonly IPAddress _ipAddress = IPAddress.Loopback;
        readonly TcpServerSocket _server = new TcpServerSocket();

        TcpClientSocket NewClient => new TcpClientSocket();

        TcpClientSocket ConnectedClient
        {
            get
            {
                var client = NewClient;
                client.Connect(_ipAddress, Port);
                Wait();
                return client;
            }
        }

        public TcpClientSocketTests()
        {
            _server.Listen(Port);
            Wait();
        }

        [Fact]
        public void DoesNotConnectToClosedPort()
        {
            var client = NewClient;
            client.Connect(_ipAddress, Port + 1);
            Wait(2000);
            client.isConnected.Should().BeFalse();
        }

        [Fact]
        public void DispatchesOnConnected()
        {
            var client = NewClient;
            var connected = 0;
            TcpClientSocket eventClient = null;

            client.OnConnected += socket =>
            {
                connected += 1;
                eventClient = socket;
            };

            client.Connect(_ipAddress, Port);
            Wait();

            connected.Should().Be(1);
            eventClient.Should().BeSameAs(client);
        }

        [Fact]
        public void Connects()
        {
            ConnectedClient.isConnected.Should().BeTrue();
        }

        [Fact]
        public void Disconnects()
        {
            var client = ConnectedClient;
            client.Disconnect();
            Wait();
            client.isConnected.Should().BeFalse();
        }

        [Fact]
        public void DispatchesOnDisconnected()
        {
            var client = ConnectedClient;
            TcpClientSocket eventSocket = null;
            client.OnDisconnected += socket => { eventSocket = socket; };
            client.Disconnect();
            Wait();
            client.isConnected.Should().BeFalse();
            eventSocket.Should().BeSameAs(client);
        }

        [Fact]
        public void DisconnectedByRemote()
        {
            var client = ConnectedClient;
            _server.Disconnect();
            Wait();
            client.isConnected.Should().BeFalse();
        }

        [Fact]
        public void DispatchesOnDisconnectedWhenDisconnectedByRemote()
        {
            var client = ConnectedClient;
            TcpClientSocket eventSocket = null;
            client.OnDisconnected += socket => { eventSocket = socket; };
            _server.Disconnect();
            Wait();
            client.isConnected.Should().BeFalse();
            eventSocket.Should().BeSameAs(client);
        }

        [Fact]
        public void ReceivesMessage()
        {
            var client = ConnectedClient;
            AbstractTcpSocket eventClient = null;
            byte[] eventBuffer = null;

            client.OnReceived += (tcpSocket, socket, b) =>
            {
                eventClient = tcpSocket;
                eventBuffer = b;
            };

            var buffer = Encoding.UTF8.GetBytes("test");
            _server.Send(buffer);

            Wait();

            eventClient.Should().BeSameAs(client);
            eventBuffer.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void SendsMessage()
        {
            var client = ConnectedClient;
            byte[] eventBytes = null;
            _server.OnReceived += (socket, socket1, bytes) => { eventBytes = bytes; };

            var buffer = Encoding.UTF8.GetBytes("test");
            client.Send(buffer);
            Wait();

            eventBytes.Should().BeEquivalentTo(buffer);
        }

        public void Dispose()
        {
            _server.Disconnect();
            Wait();
        }

        static void Wait(int ms = 50) => System.Threading.Thread.Sleep(ms);
    }
}
