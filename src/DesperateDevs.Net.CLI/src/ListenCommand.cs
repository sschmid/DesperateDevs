using System;

namespace DesperateDevs.Net.Cli
{
    public class ListenCommand : AbstractSocketCommand
    {
        public override string Trigger
        {
            get { return "listen"; }
        }

        public override string Description
        {
            get { return "Listen on port"; }
        }

        public override string Group
        {
            get { return null; }
        }

        public override string Example
        {
            get { return "listen [port]"; }
        }

        public ListenCommand() : base(typeof(ListenCommand).FullName) { }

        protected override void Run()
        {
            int port;

            try
            {
                port = int.Parse(_args[0]);
            }
            catch (Exception)
            {
                _logger.Warn("Invalid port");
                return;
            }

            var server = new TcpServerSocket();
            _socket = server;
            server.Listen(port);

            Start();
        }
    }
}
