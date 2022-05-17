using System;

namespace DesperateDevs.Net.Cli
{
    public class ConnectCommand : AbstractSocketCommand
    {
        public override string Trigger => "connect";
        public override string Description => "Connect to host on port";
        public override string Group => null;
        public override string Example => "connect [host] [port]";

        public ConnectCommand() : base(typeof(ConnectCommand).FullName) { }

        protected override void Run()
        {
            string host;
            int port;

            try
            {
                host = _args[0];
                port = int.Parse(_args[1]);
            }
            catch (Exception)
            {
                _logger.Error("Please specify a valid host and port");
                return;
            }

            var client = new TcpClientSocket();
            _socket = client;
            client.OnDisconnected += _ => Environment.Exit(0);
            client.Connect(host, port);
            Start();
        }
    }
}
