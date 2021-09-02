using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.Networking;
using DesperateDevs.Serialization;
using DesperateDevs.Serialization.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class ClientCommand : AbstractPreferencesCommand
    {
        public override string trigger => "client";
        public override string description => "Start client mode";
        public override string group => CommandGroups.CODE_GENERATION;
        public override string example => "client [command]";

        string _command;

        public ClientCommand() : base(typeof(ClientCommand).FullName)
        {
        }

        protected override void run()
        {
            _command = string.Join(" ", _rawArgs.Skip(1).ToArray());

            var config = _preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var client = new TcpClientSocket();
            client.OnConnected += onConnected;
            client.OnReceived += onReceived;
            client.OnDisconnected += onDisconnected;
            client.Connect(config.host.ResolveHost(), config.port);

            while (true)
            {
            }
        }

        void onConnected(TcpClientSocket client)
        {
            client.Send(Encoding.UTF8.GetBytes(_command));
        }

        void onReceived(AbstractTcpSocket socket, Socket client, byte[] bytes)
        {
            _logger.Info(Encoding.UTF8.GetString(bytes));
            socket.Disconnect();
        }

        void onDisconnected(AbstractTcpSocket socket)
        {
            Environment.Exit(0);
        }
    }
}
