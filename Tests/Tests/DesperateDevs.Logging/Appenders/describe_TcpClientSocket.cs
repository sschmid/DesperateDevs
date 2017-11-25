using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DesperateDevs.Logging.Appenders;
using NSpec;

class describe_TcpClientSocket : nspec {

    const int Port = 2345;
    Socket _acceptServer;
    Socket _clientServer;

    void when_created() {

        TcpClientSocket client = null;

        before = () => {
            client = new TcpClientSocket();
        };

        it["isn't connected"] = () => client.isConnected.should_be_false();

        it["disconnects without triggering event"] = () => {
            client.OnDisconnect += delegate { this.Fail(); };
            client.Disconnect();
        };

        it["can not connect when host is not available"] = () => {
            client.OnConnect += delegate { this.Fail(); };
            client.Connect(IPAddress.Loopback, Port);
            wait();
            client.isConnected.should_be_false();
        };

        it["can connect when host is available"] = () => {
            var didConnect = false;
            createServer(Port);
            client.OnConnect += delegate { didConnect = true; };
            client.Connect(IPAddress.Loopback, Port);
            wait();
            client.isConnected.should_be_true();
            didConnect.should_be_true();
            closeServer();
        };

        it["can not send"] = () => client.Send(new byte[] { 1, 2 });

        context["when connected"] = () => {

            before = () => {
                createServer(Port);
                client.Connect(IPAddress.Loopback, Port);
                wait();
            };

            after = () => {
                closeServer();
            };

            it["can disconnect"] = () => {
                var didDisconnect = false;
                client.OnDisconnect += delegate { didDisconnect = true; };
                client.Disconnect();
                wait();
                client.isConnected.should_be_false();
                didDisconnect.should_be_true();
            };

            it["receives disconnect"] = () => {
                var didDisconnect = false;
                client.OnDisconnect += delegate { didDisconnect = true; };
                closeServer();
                wait();
                didDisconnect.should_be_true();
                client.isConnected.should_be_false();
            };

            it["receives message"] = () => {
                const string message = "Hello";
                var receivedMessage = string.Empty;
                client.OnReceive += (sender, c, bytes) => receivedMessage = Encoding.UTF8.GetString(bytes);
                _clientServer.Send(Encoding.UTF8.GetBytes(message));
                wait();
                message.should_be(receivedMessage);
            };

            it["receives multiple messages"] = () => {
                const string message1 = "Hello1";
                const string message2 = "Hello2";

                byte[] bytes = null;
                client.OnReceive += (sender, c, b) => bytes = b;

                _clientServer.Send(Encoding.UTF8.GetBytes(message1));
                wait();
                message1.should_be(Encoding.UTF8.GetString(bytes));

                _clientServer.Send(Encoding.UTF8.GetBytes(message2));
                wait();
                message2.should_be(Encoding.UTF8.GetString(bytes));
            };

            it["can send"] = () => {
                const string message = "Hello";
                var receivedMessage = string.Empty;
                byte[] buffer = new byte[_clientServer.ReceiveBufferSize];
                _clientServer.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    ar => {
                        var server = (Socket)ar.AsyncState;
                        var bytesReceived = server.EndReceive(ar);
                        var trimmedBuffer = new byte[bytesReceived];
                        Array.Copy(buffer, trimmedBuffer, bytesReceived);
                        receivedMessage = Encoding.UTF8.GetString(trimmedBuffer);
                    }, _clientServer);

                client.Send(Encoding.UTF8.GetBytes(message));
                wait();
                message.should_be(receivedMessage);
            };
        };
    }

    void wait() {
        Thread.Sleep(20);
    }

    void createServer(int port) {
        _acceptServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _acceptServer.Bind(new IPEndPoint(IPAddress.Loopback, port));
        _acceptServer.Listen(0);
        _acceptServer.BeginAccept(ar => {
            var socket = (Socket)ar.AsyncState;
            _clientServer = socket.EndAccept(ar);
        }, _acceptServer);
    }

    void closeServer() {
        try {
            _clientServer.Disconnect(false);
            _clientServer.Close();
            _acceptServer.Disconnect(false);
            _acceptServer.Close();
        } catch (Exception) {
        }
    }
}
