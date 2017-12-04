using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.Networking;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Client : AbstractPreferencesCommand {

        public override string trigger { get { return "client"; } }
        public override string description { get { return "Start client mode (default port is 3333)"; } }
        public override string example { get { return "jenny client port command"; } }

        string _command;

        public Client() : base(typeof(Client).Name) {
        }

        protected override void run() {
            var port = 0;
            try {
                port = int.Parse(_rawArgs[1]);
                _command = string.Join(" ", _rawArgs.Skip(2).ToArray());
            } catch (Exception) {
                port = 3333;
                _command = string.Join(" ", _rawArgs.Skip(1).ToArray());
            }

            var client = new TcpClientSocket();
            client.OnConnected += onConnected;
            client.OnReceived += onReceived;
            client.OnDisconnected += onDisconnected;
            client.Connect(IPAddress.Parse("127.0.0.1"), port);

            while (true) {
            }
        }

        void onConnected(TcpClientSocket client) {
            client.Send(Encoding.UTF8.GetBytes(_command));
        }

        void onReceived(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            _logger.Info(Encoding.UTF8.GetString(bytes));
            socket.Disconnect();
        }

        void onDisconnected(AbstractTcpSocket socket) {
            Environment.Exit(0);
        }
    }
}
