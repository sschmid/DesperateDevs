using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.Networking;
using NSpec;

class describe_TcpServerSocket : nspec {

    const int port = 1234;
    readonly IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

    void when_socket() {

        TcpServerSocket server = null;

        before = () => {
            server = new TcpServerSocket();
        };

        after = () => {
            server.Disconnect();
            this.Wait();
        };

        it["can disconnect even when already disconnected"] = () => {
            server.Disconnect();
            server.Disconnect();
        };

        it["new server socket doesn't have connected clients"] = () => {
            server.Listen(port);
            this.Wait();
            server.count.should_be(0);
        };

        it["can send without having connected clients"] = () => {
            const string message = "Hi";
            var buffer = Encoding.Unicode.GetBytes(message);
            server.Listen(port);
            this.Wait();
            server.Send(buffer);
            this.Wait();
        };

        context["when port open"] = () => {

            Socket client1 = null;

            before = () => {
                server.Listen(port);
                client1 = createClient();
            };

            context["when client connects"] = () => {

                after = () => {
                    disconnectClient(client1);
                    this.Wait();
                };

                it["accepts client connection"] = () => {
                    client1.Connect(ipAddress, port);
                    this.Wait();

                    server.count.should_be(1);
                };

                it["triggers OnClientConnected"] = () => {
                    var connected = 0;
                    TcpServerSocket eventServer = null;
                    server.OnClientConnected += (s, c) => {
                        connected += 1;
                        eventServer = s;
                    };

                    client1.Connect(ipAddress, port);
                    this.Wait();

                    connected.should_be(1);
                    eventServer.should_be_same(server);
                };

                it["gets client by endPoint"] = () => {
                    Socket eventClient = null;
                    server.OnClientConnected += (s, c) => {
                        eventClient = c;
                    };

                    client1.Connect(ipAddress, port);
                    this.Wait();

                    var rep = (IPEndPoint)eventClient.RemoteEndPoint;
                    var key = new IPEndPoint(IPAddress.Parse(rep.Address.ToString()), rep.Port);

                    var client = server.GetClientWithRemoteEndPoint(key);
                    client.should_be_same(eventClient);
                };
            };

            context["when client connected"] = () => {

                IPEndPoint rep1 = null;

                before = () => {
                    server.OnClientConnected += (socket, client) => {
                        rep1 = (IPEndPoint)client.RemoteEndPoint;
                    };
                    client1.Connect(ipAddress, port);
                    this.Wait();
                };

                context["disconnect"] = () => {

                    it["removes disconnected client"] = () => {
                        disconnectClient(client1);
                        this.Wait();
                        server.count.should_be(0);
                    };

                    it["triggers OnClientDisconnected"] = () => {
                        var disconnected = 0;
                        TcpServerSocket eventServer = null;

                        server.OnClientDisconnected += (s, c) => {
                            disconnected += 1;
                            eventServer = s;
                        };

                        disconnectClient(client1);
                        this.Wait();

                        disconnected.should_be(1);
                        eventServer.should_be_same(server);
                    };

                    it["server disconnects and removes clients"] = () => {
                        server.Disconnect();
                        this.Wait();
                        server.count.should_be(0);
                    };

                    it["disconnects specified client"] = () => {
                        prepareForReceive(client1, s => {});
                        this.Wait();

                        var client = server.GetClientWithRemoteEndPoint(rep1);
                        client.Connected.should_be_true();

                        server.DisconnectClient(rep1);
                        this.Wait();

                        client.Connected.should_be_false();
                    };
                };

                context["communcation"] = () => {

                    after = () => {
                        disconnectClient(client1);
                        this.Wait();
                    };

                    it["receives client message"] = () => {
                        AbstractTcpSocket eventServer = null;
                        byte[] eventBytes = null;
                        server.OnReceived += (tcpSocket, socket, bytes) => {
                            eventServer = tcpSocket;
                            eventBytes = bytes;
                        };

                        const string message = "Hi";
                        var buffer = Encoding.Unicode.GetBytes(message);
                        client1.Send(buffer, 0, buffer.Length, SocketFlags.None);
                        this.Wait();

                        eventServer.should_be_same(server);
                        eventBytes.should_be(buffer);
                    };

                    it["sends message to client"] = () => {
                        string eventMsg = null;
                        prepareForReceive(client1, s => eventMsg = s);

                        const string message = "Hi";
                        var buffer = Encoding.Unicode.GetBytes(message);
                        server.Send(buffer);
                        this.Wait();

                        eventMsg.should_be(message);
                    };

                    it["receives multiple client messages"] = () => {
                        var eventBytes = new List<byte[]>();
                        server.OnReceived += (sender, receiver, bytes) => {
                            eventBytes.Add(bytes);
                        };

                        const string message1 = "Hi";
                        const string message2 = "Bye";
                        var buffer1 = Encoding.Unicode.GetBytes(message1);
                        var buffer2 = Encoding.Unicode.GetBytes(message2);
                        client1.Send(buffer1, 0, buffer1.Length, SocketFlags.None);
                        this.Wait();
                        client1.Send(buffer2, 0, buffer2.Length, SocketFlags.None);
                        this.Wait();

                        eventBytes[0].should_be(buffer1);
                        eventBytes[1].should_be(buffer2);
                    };

                    context["when another client is connected"] = () => {

                        Socket client2 = null;
                        IPEndPoint rep2 = null;

                        before = () => {
                            client2 = createClient();
                            server.OnClientConnected += (socket, client) => {
                                rep2 = (IPEndPoint)client.RemoteEndPoint;
                            };

                            client2.Connect(ipAddress, port);
                            this.Wait();
                        };

                        after = () => {
                            disconnectClient(client2);
                            this.Wait();
                        };

                        it["accepts multiple client connection"] = () => {
                            server.count.should_be(2);
                        };

                        it["sends message to all connected clients"] = () => {
                            string eventMsg1 = null;
                            string eventMsg2 = null;
                            prepareForReceive(client1, s => eventMsg1 = s);
                            prepareForReceive(client2, s => eventMsg2 = s);

                            const string message = "Hi";
                            var buffer = Encoding.Unicode.GetBytes(message);
                            server.Send(buffer);
                            this.Wait();

                            eventMsg1.should_be(message);
                            eventMsg2.should_be(message);
                        };

                        it["sends message to specified client"] = () => {
                            string eventMsg1 = null;
                            string eventMsg2 = null;
                            prepareForReceive(client1, s => eventMsg1 = s, true);
                            prepareForReceive(client2, s => eventMsg2 = s);

                            const string message = "Hi";
                            var buffer = Encoding.Unicode.GetBytes(message);
                            server.SendTo(buffer, rep2);
                            this.Wait();

                            eventMsg1.should_be_null();
                            eventMsg2.should_be(message);
                        };
                    };
                };
            };
        };
    }

    Socket createClient() {
        var ipe = new IPEndPoint(ipAddress, port);
        var client = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        return client;
    }

    void disconnectClient(Socket client) {
        client.Shutdown(SocketShutdown.Both);
        client.Disconnect(false);
        client.Close();
    }

    void prepareForReceive(Socket socket, Action<string> onReceive, bool mayFail = false, Action failAction = null) {
        var buffer = new byte[socket.ReceiveBufferSize];
        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
            ar => {
                var client = (Socket)ar.AsyncState;

                if (mayFail) {
                    try {
                        var bytesReceived = client.EndReceive(ar);
                        var trimmedBuffer = new byte[bytesReceived];
                        Array.Copy(buffer, trimmedBuffer, bytesReceived);
                        onReceive(Encoding.Unicode.GetString(trimmedBuffer));
                    } catch (Exception) {
                        if (failAction != null) {
                            failAction();
                        }
                    }
                } else {
                    var bytesReceived = client.EndReceive(ar);
                    var trimmedBuffer = new byte[bytesReceived];
                    Array.Copy(buffer, trimmedBuffer, bytesReceived);
                    onReceive(Encoding.Unicode.GetString(trimmedBuffer));
                }
            }, socket);
    }
}
