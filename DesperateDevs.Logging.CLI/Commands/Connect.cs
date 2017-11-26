using System;
using System.Net;
using DesperateDevs.Logging.Appenders;

namespace DesperateDevs.Logging.CLI {

    public class Connect : AbstractSocketCommand {

        public override string trigger { get { return "connect"; } }
        public override string description { get { return "Connect to ip on port"; } }
        public override string example { get { return "fabl connect [ip] [port]"; } }

        protected override void run() {
            try {
                var ip = IPAddress.Parse(_args[1]);
                var port = int.Parse(_args[2]);
                var client = new TcpClientSocket();
                _socket = client;
                client.OnDisconnect += onDisconnect;
                client.Connect(ip, port);
                base.run();
            } catch (Exception) {
                fabl.Warn("Please specify a valid ip and port");
            }
        }
    }
}
