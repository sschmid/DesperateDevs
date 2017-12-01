using System;
using System.Net;
using DesperateDevs.Logging;

namespace DesperateDevs.Networking.CLI {

    public class Connect : AbstractSocketCommand {

        public override string trigger { get { return "connect"; } }
        public override string description { get { return "Connect to ip on port"; } }
        public override string example { get { return "pezy connect [ip port]"; } }

        protected override void run() {
            IPAddress ip = null;
            int port;

            try {
                ip = IPAddress.Parse(_args[0]);
                port = int.Parse(_args[1]);
            } catch (Exception) {
                fabl.Warn("Please specify a valid ip and port");
                return;
            }

            var client = new TcpClientSocket();
            _socket = client;
            client.OnDisconnected += socket => Environment.Exit(0);
            client.Connect(ip, port);

            start();
        }
    }
}
