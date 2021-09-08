// TODO don't run tests in parallel
// using System;
// using System.Net;
// using System.Text;
// using DesperateDevs.Tests;
// using FluentAssertions;
// using Xunit;
//
// namespace DesperateDevs.Networking.Tests
// {
//     public class TcpClientSocketTests : IDisposable
//     {
//         const int Port = 4321;
//
//         readonly IPAddress _ipAddress = IPAddress.Loopback;
//         readonly TcpServerSocket _server = new();
//
//         TcpClientSocket NewClient => new();
//
//         TcpClientSocket ConnectedClient
//         {
//             get
//             {
//                 var client = NewClient;
//                 client.Connect(_ipAddress, Port);
//                 TestHelper.Wait();
//                 return client;
//             }
//         }
//
//         public TcpClientSocketTests()
//         {
//             _server.Listen(Port);
//             TestHelper.Wait();
//         }
//
//         [Fact]
//         public void DoesNotConnectToClosedPort()
//         {
//             var client = NewClient;
//             client.Connect(_ipAddress, Port);
//             TestHelper.Wait(2000);
//             client.isConnected.Should().BeFalse();
//         }
//
//         [Fact]
//         public void DispatchesOnConnected()
//         {
//             var client = NewClient;
//             var connected = 0;
//             TcpClientSocket eventClient = null;
//
//             client.OnConnected += socket =>
//             {
//                 connected += 1;
//                 eventClient = socket;
//             };
//
//             client.Connect(_ipAddress, Port);
//             TestHelper.Wait();
//
//             connected.Should().Be(1);
//             eventClient.Should().BeSameAs(client);
//         }
//
//         [Fact]
//         public void Connects()
//         {
//             ConnectedClient.isConnected.Should().BeTrue();
//         }
//
//         [Fact]
//         public void Disconnects()
//         {
//             var client = ConnectedClient;
//             client.Disconnect();
//             TestHelper.Wait();
//             client.isConnected.Should().BeFalse();
//         }
//
//         [Fact]
//         public void DispatchesOnDisconnected()
//         {
//             var client = ConnectedClient;
//             TcpClientSocket eventSocket = null;
//             client.OnDisconnected += socket => { eventSocket = socket; };
//             client.Disconnect();
//             TestHelper.Wait();
//             client.isConnected.Should().BeFalse();
//             eventSocket.Should().BeSameAs(client);
//         }
//
//         [Fact]
//         public void DisconnectedByRemote()
//         {
//             var client = ConnectedClient;
//             _server.Disconnect();
//             TestHelper.Wait();
//             client.isConnected.Should().BeFalse();
//         }
//
//         [Fact]
//         public void DispatchesOnDisconnectedWhenDisconnectedByRemote()
//         {
//             var client = ConnectedClient;
//             TcpClientSocket eventSocket = null;
//             client.OnDisconnected += socket => { eventSocket = socket; };
//             _server.Disconnect();
//             TestHelper.Wait();
//             client.isConnected.Should().BeFalse();
//             eventSocket.Should().BeSameAs(client);
//         }
//
//         [Fact]
//         public void ReceivesMessage()
//         {
//             var client = ConnectedClient;
//             AbstractTcpSocket eventClient = null;
//             byte[] eventBuffer = null;
//
//             client.OnReceived += (tcpSocket, socket, b) =>
//             {
//                 eventClient = tcpSocket;
//                 eventBuffer = b;
//             };
//
//             var buffer = Encoding.UTF8.GetBytes("test");
//             _server.Send(buffer);
//
//             TestHelper.Wait();
//
//             eventClient.Should().BeSameAs(client);
//             eventBuffer.Should().BeEquivalentTo(buffer);
//         }
//
//         [Fact]
//         public void SendsMessage()
//         {
//             var client = ConnectedClient;
//             byte[] eventBytes = null;
//             _server.OnReceived += (socket, socket1, bytes) => { eventBytes = bytes; };
//
//             var buffer = Encoding.UTF8.GetBytes("test");
//             client.Send(buffer);
//             TestHelper.Wait();
//
//             eventBytes.Should().BeEquivalentTo(buffer);
//         }
//
//         public void Dispose()
//         {
//             _server.Disconnect();
//             TestHelper.Wait();
//         }
//     }
// }
