using System;
using System.Net;

namespace DesperateDevs.Networking.CLI {

    public class Connect : AbstractSocketCommand {

        public override string trigger { get { return "connect"; } }
        public override string description { get { return "Connect to ip on port"; } }
        public override string example { get { return "connect [ip port]"; } }

        public Connect() : base(typeof(Connect).FullName) {
        }

        protected override void run() {
            IPAddress ip = null;
            int port;

            try {
                ip = _args[0].ResolveHost();
                port = int.Parse(_args[1]);
            } catch (Exception) {
                _logger.Warn("Please specify a valid ip and port");
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
