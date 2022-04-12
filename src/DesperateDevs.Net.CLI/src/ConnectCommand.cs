using System;
using System.Net;

namespace DesperateDevs.Net.Cli
{
    public class ConnectCommand : AbstractSocketCommand
    {
        public override string Trigger => "connect";
        public override string Description => "Connect to ip on port";
        public override string Group => null;
        public override string Example => "connect [ip] [port]";

        public ConnectCommand() : base(typeof(ConnectCommand).FullName) { }

        protected override void Run()
        {
            IPAddress ip;
            int port;

            try
            {
                ip = _args[0].ResolveHost();
                port = int.Parse(_args[1]);
            }
            catch (Exception)
            {
                _logger.Warn("Please specify a valid ip and port");
                return;
            }

            var client = new TcpClientSocket();
            _socket = client;
            client.OnDisconnected += socket => Environment.Exit(0);
            client.Connect(ip, port);
            Start();
        }
    }
}
