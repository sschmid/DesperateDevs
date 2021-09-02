using System;

namespace DesperateDevs.Networking.CLI {

    public class ListenCommand : AbstractSocketCommand {

        public override string trigger { get { return "listen"; } }
        public override string description { get { return "Listen on port"; } }
        public override string group { get { return null; } }
        public override string example { get { return "listen [port]"; } }

        public ListenCommand() : base(typeof(ListenCommand).FullName) {
        }

        protected override void run() {
            int port;

            try {
                port = int.Parse(_args[0]);
            } catch (Exception) {
                _logger.Warn("Invalid port");
                return;
            }

            var server = new TcpServerSocket();
            _socket = server;
            server.Listen(port);

            start();
        }
    }
}
