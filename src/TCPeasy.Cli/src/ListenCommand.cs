using System;

namespace TCPeasy.Cli
{
    public class ListenCommand : AbstractSocketCommand
    {
        public override string Trigger => "listen";
        public override string Description => "Listen on port";
        public override string Group => null;
        public override string Example => "listen [port]";

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
                _logger.Error("Invalid port");
                return;
            }

            var server = new TcpServerSocket();
            _socket = server;
            server.Listen(port);
            Start();
        }
    }
}
