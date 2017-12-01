using System;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.CLI;
using DesperateDevs.Logging;

namespace DesperateDevs.Networking.CLI {

    public abstract class AbstractSocketCommand : AbstractCommand {

        public abstract override string trigger { get; }
        public abstract override string description { get; }
        public abstract override string example { get; }

        protected AbstractTcpSocket _socket;

        protected void start() {
            _socket.OnReceived += onReceive;
            Console.CancelKeyPress += onCancel;
            while (true) {
                _socket.Send(Encoding.Unicode.GetBytes(Console.ReadLine()));
            }
        }

        protected void onReceive(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            fabl.Info(Encoding.Unicode.GetString(bytes));
        }

        protected void onCancel(object sender, ConsoleCancelEventArgs e) {
            _socket.Disconnect();
        }
    }
}
