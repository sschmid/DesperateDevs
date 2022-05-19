using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Sherlog;
using Sherlog.Formatters;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace TCPeasy.Tests
{
    [Collection("Non-Parallel")]
    public class TcpClientSocketTests : IDisposable
    {
        static readonly LogMessageFormatter Formatter = new LogMessageFormatter("[{1}]\t{0}: {2}");

        // while true; do nc -l 4321; done
        const int Port = 4321;

        readonly IPAddress _ipAddress = IPAddress.Loopback;

        readonly ITestOutputHelper _output;
        readonly List<(LogLevel LogLevel, string Message)> _logs;

        TcpServerSocket _server = new TcpServerSocket();
        TcpClientSocket _client;

        public TcpClientSocketTests(ITestOutputHelper output)
        {
            _output = output;
            _logs = new List<(LogLevel, string)>();
            Logger.AddAppender((logger, level, message) => _output.WriteLine(Formatter.FormatMessage(logger, level, message)));
            Logger.AddAppender((_, level, message) => _logs.Add((level, message)));
            _server.Listen(Port);
            Wait();
        }

        [Fact]
        public void ConnectsToHost()
        {
            CreateClient();
            _client.Connect("127.0.0.1", Port);
            Wait();
            _client.IsConnected.Should().BeTrue();
        }

        [Fact]
        public void ConnectsToIpAddress()
        {
            CreateConnectedClient().IsConnected.Should().BeTrue();
        }

        [Fact]
        public void DispatchesOnConnected()
        {
            CreateClient();
            (TcpClientSocket Client, int Invoked) args = new(null, 0);
            _client.OnConnected += socket =>
            {
                args.Client = socket;
                args.Invoked += 1;
            };

            Connect();
            args.Invoked.Should().Be(1);
            args.Client.Should().BeSameAs(_client);
        }

        [Fact]
        public void DoesNotConnectToClosedPort()
        {
            CreateClient();
            _client.Connect(_ipAddress, Port + 1);
            Wait();
            _client.IsConnected.Should().BeFalse();
            _logs.Single(log => log.LogLevel == LogLevel.Error).Message
                .Should().Be(SocketError.ConnectionRefused.ToString());
        }

        [Fact]
        public void Disconnects()
        {
            CreateConnectedClient();
            Disconnect();
            _client.IsConnected.Should().BeFalse();
        }

        [Fact]
        public void DispatchesOnDisconnected()
        {
            CreateConnectedClient();
            (TcpClientSocket Client, int Invoked) args = new(null, 0);
            _client.OnDisconnected += socket =>
            {
                args.Client = socket;
                args.Invoked += 1;
            };

            Disconnect();
            args.Invoked.Should().Be(1);
            args.Client.Should().BeSameAs(_client);
        }

        [Fact]
        public void ReusesSocket()
        {
            CreateConnectedClient();
            _client.IsConnected.Should().BeTrue();
            Disconnect();
            _client.IsConnected.Should().BeFalse();
            Connect();
            _client.IsConnected.Should().BeTrue();
        }

        [Fact]
        public void ReceivesMessage()
        {
            CreateConnectedClient();

            (AbstractTcpSocket Client, Socket Socket, byte[] Bytes, int Invoked) args = new(null, null, null, 0);
            _client.OnReceived += (tcpSocket, socket, bytes) =>
            {
                args.Client = tcpSocket;
                args.Socket = socket;
                args.Bytes = bytes;
                args.Invoked += 1;
            };

            var buffer = Send(_server, "test-message");
            args.Invoked.Should().Be(1);
            args.Client.Should().BeSameAs(_client);
            args.Bytes.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void ReceivesMultipleMessage()
        {
            CreateConnectedClient();

            (AbstractTcpSocket Client, Socket Socket, List<byte[]> BytesList) args = new(null, null, new List<byte[]>());
            _client.OnReceived += (tcpSocket, socket, bytes) =>
            {
                args.Client = tcpSocket;
                args.Socket = socket;
                args.BytesList.Add(bytes);
            };

            var buffer1 = Send(_server, "test-message-1");
            var buffer2 = Send(_server, "test-message-2");
            args.BytesList.Count.Should().Be(2);
            args.BytesList[0].Should().BeEquivalentTo(buffer1);
            args.BytesList[1].Should().BeEquivalentTo(buffer2);
        }

        [Fact]
        public void DisconnectedByRemote()
        {
            CreateConnectedClient();
            _server.Disconnect();
            Wait();
            _client.IsConnected.Should().BeFalse();
        }

        [Fact]
        public void DispatchesOnDisconnectedWhenDisconnectedByRemote()
        {
            CreateConnectedClient();

            TcpClientSocket eventSocket = null;
            _client.OnDisconnected += socket => { eventSocket = socket; };

            _server.Disconnect();
            Wait();
            _client.IsConnected.Should().BeFalse();
            eventSocket.Should().BeSameAs(_client);
        }

        [Fact]
        public void ReusesSocketDisconnectedByRemote()
        {
            CreateConnectedClient();
            _client.IsConnected.Should().BeTrue();

            _server.Disconnect();
            Wait();
            _client.IsConnected.Should().BeFalse();

            _server = new TcpServerSocket();
            _server.Listen(Port);
            Wait();

            Connect();
            _client.IsConnected.Should().BeTrue();

            (AbstractTcpSocket Client, Socket Socket, byte[] Bytes, int Invoked) args = new(null, null, null, 0);
            _client.OnReceived += (tcpSocket, socket, bytes) =>
            {
                args.Client = tcpSocket;
                args.Socket = socket;
                args.Bytes = bytes;
                args.Invoked += 1;
            };

            var buffer = Send(_server, "test-message");
            args.Invoked.Should().Be(1);
            args.Client.Should().BeSameAs(_client);
            args.Bytes.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void SendsMessage()
        {
            CreateConnectedClient();
            byte[] eventBytes = null;
            _server.OnReceived += (_, _, bytes) => eventBytes = bytes;

            var buffer = Send(_client, "test-message");
            eventBytes.Should().BeEquivalentTo(buffer);
        }

        void CreateClient() => _client = new TcpClientSocket();

        TcpClientSocket CreateConnectedClient()
        {
            CreateClient();
            Connect();
            return _client;
        }

        void Connect()
        {
            _client.Connect(_ipAddress, Port);
            Wait();
        }

        byte[] Send(AbstractTcpSocket socket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            socket.Send(buffer);
            Wait();
            return buffer;
        }

        void Disconnect()
        {
            _client.Disconnect();
            Wait();
        }

        public void Dispose()
        {
            _output.WriteLine("Dispose");
            _client.Dispose();
            _server.Disconnect();
            Wait();
            Logger.ResetAppenders();
            Logger.ResetLoggers();
        }

        static void Wait() => System.Threading.Thread.Sleep(50);
    }
}
