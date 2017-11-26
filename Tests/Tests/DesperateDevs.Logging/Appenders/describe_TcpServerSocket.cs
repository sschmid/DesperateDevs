using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DesperateDevs.Logging.Appenders;
using NSpec;

class describe_TcpServerSocket : nspec {

    const int port = 1234;

    void when_created() {

        TcpServerSocket server = null;

        before = () => {
            server = new TcpServerSocket();
        };

        it["has no connected clients"] = () => server.connectedClients.should_be(0);

        it["is not listening"] = () => server.isConnected.should_be_false();

        it["can disconnect without triggering event"] = () => {
            server.OnDisconnect += delegate { this.Fail(); };
            server.Disconnect();
        };

        it["can listen"] = () => {
            server.Listen(port);
            server.isConnected.should_be_true();
            // Cleanup
            server.Disconnect();
        };

        it["cannot listen when address is used"] = () => {
            var blockingServer = new TcpServerSocket();
            blockingServer.Listen(port);

            server.Listen(port);
            server.isConnected.should_be_false();

            // Cleanup
            blockingServer.Disconnect();
        };

        it["cannot send"] = () => server.Send(new byte[] { 1, 2 });

        context["when listening"] = () => {

            before = () => {
                server.Listen(port);
            };

            after = () => {
                server.Disconnect();
            };

            it["can disconnect"] = () => {
                var didDisconnect = false;
                server.OnDisconnect += delegate { didDisconnect = true; };
                server.Disconnect();
                didDisconnect.should_be_true();
                server.isConnected.should_be_false();
            };

            it["accepts connections"] = () => {
                var clientConnected = false;
                server.OnClientConnect += (sender, e) => clientConnected = true;
                createAndConnectClient(port);
                server.connectedClients.should_be(1);
                clientConnected.should_be_true();
            };

            it["accepts multiple connections"] = () => {
                createAndConnectClient(port);
                createAndConnectClient(port);
                createAndConnectClient(port);
                server.connectedClients.should_be(3);
            };

            context["when connection accepted"] = () => {

                Socket client1 = null;
                Socket client2 = null;

                before = () => {
                    client1 = createAndConnectClient(port);
                    client2 = createAndConnectClient(port);
                };

                it["can disconnect"] = () => {
                    server.Disconnect();
                    wait();
                    server.connectedClients.should_be(0);
                };

                it["receives client disconnect"] = () => {
                    var clientDidDisconntect = false;
                    server.OnReceive += delegate { this.Fail(); };
                    server.OnClientDisconnect += (sender, e) => clientDidDisconntect = true;
                    client1.Disconnect(false);
                    client1.Close();
                    wait();
                    server.connectedClients.should_be(1);
                    clientDidDisconntect.should_be_true();
                };

                it["receives message"] = () => {
                    var message = "Hello";
                    byte[] bytes = null;
                    Socket cl = null;
                    server.OnReceive += (socket, c, b) => { bytes = b; cl = c; };
                    client1.Send(Encoding.UTF8.GetBytes(message));
                    wait();
                    message.should_be(Encoding.UTF8.GetString(bytes));
                    cl.should_not_be_null();
                };

                it["receives multiple messages"] = () => {
                    var message1 = "Hello1";
                    var message2 = "Hello2";
                    byte[] bytes = null;
                    server.OnReceive += (socket, c, b) => bytes = b;

                    client1.Send(Encoding.UTF8.GetBytes(message1));
                    wait();
                    message1.should_be(Encoding.UTF8.GetString(bytes));

                    client1.Send(Encoding.UTF8.GetBytes(message2));
                    wait();
                    message2.should_be(Encoding.UTF8.GetString(bytes));
                };

                it["can respond to client"] = () => {
                    var clientMessage = "Hello";
                    var serverMessage = "Hi";
                    var receivedMessage = string.Empty;
                    Socket cl = null;
                    server.OnReceive += (socket, c, b) => cl = c;
                    client1.Send(Encoding.UTF8.GetBytes(clientMessage));
                    wait();

                    prepareForReceive(client1, msg => receivedMessage = msg);

                    server.SendWith(cl, Encoding.UTF8.GetBytes(serverMessage));
                    wait();

                    receivedMessage.should_be(serverMessage);
                };

                it["can send to all connected clients"] = () => {
                    var message = "Hello";
                    var client1ReceivedMessage = string.Empty;
                    var client2ReceivedMessage = string.Empty;
                    prepareForReceive(client1, msg => client1ReceivedMessage = msg);
                    prepareForReceive(client2, msg => client2ReceivedMessage = msg);

                    server.Send(Encoding.UTF8.GetBytes(message));
                    wait();

                    client1ReceivedMessage.should_be(message);
                    client2ReceivedMessage.should_be(message);
                };
            };
        };
    }

    void wait() {
        Thread.Sleep(50);
    }

    Socket createAndConnectClient(int port) {
        var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IPAddress.Loopback, port);
        wait();
        return client;
    }

    void prepareForReceive(Socket socket, Action<string> onReceive) {
        var buffer = new byte[socket.ReceiveBufferSize];
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
            ar => {
                var client = (Socket)ar.AsyncState;
                var bytesReceived = client.EndReceive(ar);
                var trimmedBuffer = new byte[bytesReceived];
                Array.Copy(buffer, trimmedBuffer, bytesReceived);
                onReceive(Encoding.UTF8.GetString(trimmedBuffer));
            }, socket);
    }
}
