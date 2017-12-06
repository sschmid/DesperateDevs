using System.Net;
using System.Text;
using DesperateDevs.Networking;
using NSpec;

class describe_TcpClientSocket : nspec {

    const int port = 4321;
    IPAddress ipAddress = IPAddress.Loopback;

    void when_socket() {

        xcontext["socket"] = () => {

            TcpClientSocket client = null;

            before = () => {
                client = new TcpClientSocket();
            };

            it["doesn't conntect to closed port"] = () => {
                client.Connect(ipAddress, port);
                this.Wait(2000);
                client.isConnected.should_be_false();
            };

            context["when open port"] = () => {

                TcpServerSocket server = null;

                before = () => {
                    server = new TcpServerSocket();
                    server.Listen(port);
                    this.Wait();
                };

                after = () => {
                    server.Disconnect();
                    this.Wait();
                };

                it["triggers OnConnected"] = () => {
                    var connected = 0;
                    TcpClientSocket eventClient = null;

                    client.OnConnected += socket => {
                        connected += 1;
                        eventClient = socket;
                    };

                    client.Connect(ipAddress, port);
                    this.Wait();

                    connected.should_be(1);
                    eventClient.should_be_same(client);
                };

                context["when connected"] = () => {

                    before = () => {
                        client.Connect(ipAddress, port);
                        this.Wait();
                    };

                    it["is conntected"] = () => {
                        client.isConnected.should_be_true();
                    };

                    it["disconnects"] = () => {
                        client.Disconnect();
                        this.Wait();
                        client.isConnected.should_be_false();
                    };

                    it["triggers OnDisconnected"] = () => {
                        TcpClientSocket eventSocket = null;
                        client.OnDisconnected += socket => {
                            eventSocket = socket;
                        };
                        client.Disconnect();
                        this.Wait();
                        client.isConnected.should_be_false();
                        eventSocket.should_be_same(client);
                    };

                    it["disconnteced by remote"] = () => {
                        server.Disconnect();
                        this.Wait();
                        client.isConnected.should_be_false();
                    };

                    it["triggers OnDisconnected when disconnteced by remote"] = () => {
                        TcpClientSocket eventSocket = null;
                        client.OnDisconnected += socket => {
                            eventSocket = socket;
                        };
                        server.Disconnect();
                        this.Wait();
                        client.isConnected.should_be_false();
                        eventSocket.should_be_same(client);
                    };

                    it["receives message"] = () => {
                        AbstractTcpSocket eventClient = null;
                        byte[] eventBuffer = null;

                        client.OnReceived += (tcpSocket, socket, b) => {
                            eventClient = tcpSocket;
                            eventBuffer = b;
                        };

                        const string message = "Hi";
                        var buffer = Encoding.UTF8.GetBytes(message);
                        server.Send(buffer);

                        this.Wait();

                        eventClient.should_be_same(client);
                        eventBuffer.should_be(buffer);
                    };

                    it["sends a message"] = () => {
                        byte[] eventBytes = null;
                        server.OnReceived += (socket, socket1, bytes) => {
                            eventBytes = bytes;
                        };

                        const string message = "Hi";
                        var buffer = Encoding.UTF8.GetBytes(message);
                        client.Send(buffer);
                        this.Wait();

                        eventBytes.should_be(buffer);
                    };
                };
            };
        };
    }
}
