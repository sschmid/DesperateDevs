using System;
using DesperateDevs.Logging.Appenders;

namespace DesperateDevs.Logging.CLI {

    public class Listen : AbstractSocketCommand {

        public override string trigger { get { return "listen"; } }
        public override string description { get { return "Listen on port"; } }
        public override string example { get { return "fabl listen [port]"; } }

        protected override void run() {
            try {
                var port = int.Parse(_args[1]);
                var server = new TcpServerSocket();
                _socket = server;
                server.OnDisconnect += onDisconnect;
                server.Listen(port);
                base.run();
            } catch (Exception) {
                fabl.Warn("Invalid port");
            }
        }
    }
}
