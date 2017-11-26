using System;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.CLI;
using DesperateDevs.Logging.Appenders;

namespace DesperateDevs.Logging.CLI {

    public abstract class AbstractSocketCommand : AbstractCommand {

        public abstract override string trigger { get; }
        public abstract override string description { get; }
        public abstract override string example { get; }

        protected AbstractTcpSocket _socket;

        protected override void run() {
            _socket.OnReceive += onReceive;
            Console.CancelKeyPress += onCancel;
            while (true) {
                _socket.Send(Encoding.UTF8.GetBytes(Console.ReadLine()));
            }
        }

        protected void onReceive(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            fabl.Info(Encoding.UTF8.GetString(bytes));
        }

        protected void onCancel(object sender, ConsoleCancelEventArgs e) {
            _socket.Disconnect();
        }

        protected void onDisconnect(AbstractTcpSocket socket) {
            Environment.Exit(0);
        }
    }
}
