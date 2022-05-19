using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FluentAssertions;
using Sherlog;
using Sherlog.Formatters;
using Xunit;
using Xunit.Abstractions;

namespace TCPeasy.Tests
{
    [Collection("Non-Parallel")]
    public class TcpServerSocketTests : IDisposable
    {
        static readonly LogMessageFormatter Formatter = new LogMessageFormatter("[{1}]\t{0}: {2}");

        const int Port = 1234;
        static readonly IPAddress IpAddress = IPAddress.Loopback;

        readonly TcpServerSocket _server = new TcpServerSocket();

        Socket _client1;
        Socket _client2;

        readonly ITestOutputHelper _output;

        public TcpServerSocketTests(ITestOutputHelper output)
        {
            _output = output;
            Logger.AddAppender((logger, level, message) => _output.WriteLine(Formatter.FormatMessage(logger, level, message)));
        }

        [Fact]
        public void NewServerSocketDoesNotHaveConnectedClients()
        {
            Listen();
            _server.Count.Should().Be(0);
        }

        [Fact]
        public void AcceptsClientConnection()
        {
            Listen();
            ConnectClient(out _client1);
            _server.Count.Should().Be(1);
        }

        [Fact]
        public void DispatchesOnClientConnected()
        {
            (TcpServerSocket Server, Socket Client, int Invoked) args = new(null, null, 0);
            _server.OnClientConnected += (server, client) =>
            {
                args.Server = server;
                args.Client = client;
                args.Invoked += 1;
            };

            Listen();

            ConnectClient(out _client1);
            args.Invoked.Should().Be(1);
            var rep = (IPEndPoint)args.Client.RemoteEndPoint!;
            rep.Port.Should().Be(((IPEndPoint)_client1.LocalEndPoint)!.Port);
            args.Server.Should().BeSameAs(_server);
        }

        [Fact]
        public void AcceptsMultipleClientConnection()
        {
            Listen();
            ConnectClient(out _client1);
            ConnectClient(out _client2);
            _server.Count.Should().Be(2);
        }

        [Fact]
        public void DispatchesMultipleOnClientConnected()
        {
            (TcpServerSocket Server, Socket Client, int Invoked) args = new(null, null, 0);
            _server.OnClientConnected += (server, client) =>
            {
                args.Server = server;
                args.Client = client;
                args.Invoked += 1;
            };

            Listen();

            ConnectClient(out _client1);
            args.Invoked.Should().Be(1);
            var rep1 = (IPEndPoint)args.Client.RemoteEndPoint!;
            rep1.Port.Should().Be(((IPEndPoint)_client1.LocalEndPoint)!.Port);
            args.Server.Should().BeSameAs(_server);

            ConnectClient(out _client2);
            args.Invoked.Should().Be(2);
            var rep2 = (IPEndPoint)args.Client.RemoteEndPoint!;
            rep2.Port.Should().Be(((IPEndPoint)_client2.LocalEndPoint)!.Port);
            args.Server.Should().BeSameAs(_server);
        }

        [Fact]
        public void GetsClientByEndPoint()
        {
            (TcpServerSocket Server, Socket Client, int Invoked) args = new(null, null, 0);
            _server.OnClientConnected += (server, client) =>
            {
                args.Server = server;
                args.Client = client;
                args.Invoked += 1;
            };

            Listen();

            ConnectClient(out _client1);
            var rep = (IPEndPoint)args.Client.RemoteEndPoint!;
            var key = new IPEndPoint(rep.Address, rep.Port);
            var client = _server.GetClientWithRemoteEndPoint(key);
            client.Should().BeSameAs(args.Client);
        }

        [Fact]
        public void ReceivesClientMessage()
        {
            (AbstractTcpSocket Server, Socket Client, byte[] Bytes) args = new(null, null, null);
            _server.OnReceived += (server, socket, bytes) =>
            {
                args.Server = server;
                args.Client = socket;
                args.Bytes = bytes;
            };

            Listen();
            ConnectClient(out _client1);

            var buffer = SendUsingClient(_client1);
            args.Server.Should().BeSameAs(_server);
            args.Bytes.Should().BeEquivalentTo(buffer);
        }

        [Fact]
        public void ReceivesMultipleClientMessages()
        {
            (AbstractTcpSocket Server, Socket Client, List<byte[]> BytesList) args = new(null, null, new List<byte[]>());
            _server.OnReceived += (server, socket, bytes) =>
            {
                args.Server = server;
                args.Client = socket;
                args.BytesList.Add(bytes);
            };

            Listen();
            ConnectClient(out _client1);

            var buffer1 = SendUsingClient(_client1, "test-message-1");
            var buffer2 = SendUsingClient(_client1, "test-message-2");
            args.BytesList.Count.Should().Be(2);
            args.BytesList[0].Should().BeEquivalentTo(buffer1);
            args.BytesList[1].Should().BeEquivalentTo(buffer2);
        }

        [Fact]
        public void RemovesDisconnectedClient()
        {
            Listen();
            ConnectClient(out _client1);
            DisconnectClient(ref _client1);
            _server.Count.Should().Be(0);
        }

        [Fact]
        public void DispatchesOnClientDisconnected()
        {
            (TcpServerSocket Server, Socket Client, int Invoked) args = new(null, null, 0);
            _server.OnClientDisconnected += (server, client) =>
            {
                args.Server = server;
                args.Client = client;
                args.Invoked += 1;
            };

            Listen();
            ConnectClient(out _client1);

            DisconnectClient(ref _client1);
            args.Invoked.Should().Be(1);
            args.Server.Should().BeSameAs(_server);
        }

        [Fact]
        public void DoesNotSendWhenNoClients()
        {
            Listen();
            Send("test-message");
        }

        [Fact]
        public void SendsMessageToClient()
        {
            string received = null;
            Listen();
            ConnectClient(out _client1);
            PrepareForReceive(_client1, s => received = s);
            const string message = "test-message";

            Send(message);
            received.Should().Be(message);
        }

        [Fact]
        public void SendsMessageToAllConnectedClients()
        {
            Listen();
            ConnectClient(out _client1);
            ConnectClient(out _client2);
            string received1 = null;
            string received2 = null;
            PrepareForReceive(_client1, s => received1 = s);
            PrepareForReceive(_client2, s => received2 = s);
            const string message = "test-message";

            Send(message);
            received1.Should().Be(message);
            received2.Should().Be(message);
        }

        [Fact]
        public void SendsMessageToSpecifiedClient()
        {
            Listen();

            ConnectClient(out _client1);

            IPEndPoint endPoint = null;
            _server.OnClientConnected += (server, client) => endPoint = (IPEndPoint)client.RemoteEndPoint;
            ConnectClient(out _client2);

            string received1 = null;
            string received2 = null;
            PrepareForReceive(_client1, s => received1 = s);
            PrepareForReceive(_client2, s => received2 = s);
            const string message = "test-message";

            SendTo(endPoint, message);
            received1.Should().BeNull();
            received2.Should().Be(message);
        }

        [Fact]
        public void DisconnectingWithoutClientsDoesNothing()
        {
            Disconnect();
            Disconnect();
        }

        [Fact]
        public void DisconnectsAndRemovesClients()
        {
            Listen();
            ConnectClient(out _client1);
            Disconnect();
            _server.Count.Should().Be(0);
        }

        [Fact]
        public void DisconnectsClient()
        {
            Listen();

            IPEndPoint endPoint = null;
            _server.OnClientConnected += (server, client) => endPoint = (IPEndPoint)client.RemoteEndPoint;
            ConnectClient(out _client1);

            var client = _server.GetClientWithRemoteEndPoint(endPoint);
            client.Connected.Should().BeTrue();

            _server.DisconnectClient(endPoint);
            Wait();

            client.Connected.Should().BeFalse();
        }

        [Fact]
        public void DisconnectsClientAndDispatchesOnClientDisconnected()
        {
            Listen();

            IPEndPoint endPoint = null;
            _server.OnClientConnected += (server, client) => endPoint = (IPEndPoint)client.RemoteEndPoint;
            (TcpServerSocket Server, Socket Client, int Invoked) args = new(null, null, 0);
            _server.OnClientDisconnected += (server, client) =>
            {
                args.Server = server;
                args.Client = client;
                args.Invoked += 1;
            };

            ConnectClient(out _client1);
            _server.DisconnectClient(endPoint);
            Wait();

            args.Invoked.Should().Be(1);
            args.Server.Should().BeSameAs(_server);
        }

        void Listen() => _server.Listen(Port);

        void Disconnect()
        {
            _server.Disconnect();
            Wait();
        }

        void ConnectClient(out Socket client)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(IpAddress, Port);
            Wait();
        }

        void Send(string message)
        {
            _server.Send(Encoding.UTF8.GetBytes(message));
            Wait();
        }

        void SendTo(IPEndPoint endPoint, string message)
        {
            _server.SendTo(Encoding.UTF8.GetBytes(message), endPoint);
            Wait();
        }

        byte[] SendUsingClient(Socket client, string message = "test-message")
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            Wait();
            return buffer;
        }

        void DisconnectClient(ref Socket client)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Disconnect(false);
            client.Close();
            Wait();
            client = null;
        }

        void PrepareForReceive(Socket socket, Action<string> onReceive)
        {
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(new byte[socket.ReceiveBufferSize], 0, socket.ReceiveBufferSize);
            args.Completed += (sender, eventArgs) =>
            {
                if (eventArgs.SocketError == SocketError.Success)
                {
                    var trimmedBuffer = new byte[eventArgs.BytesTransferred];
                    Array.Copy(eventArgs.Buffer, trimmedBuffer, eventArgs.BytesTransferred);
                    onReceive(Encoding.UTF8.GetString(trimmedBuffer));
                }
            };
            socket.ReceiveAsync(args);
            Wait();
        }

        public void Dispose()
        {
            _output.WriteLine("Dispose");
            Disconnect();
            Wait();
            Logger.ResetAppenders();
            Logger.ResetLoggers();
        }

        static void Wait() => System.Threading.Thread.Sleep(50);
    }
}
