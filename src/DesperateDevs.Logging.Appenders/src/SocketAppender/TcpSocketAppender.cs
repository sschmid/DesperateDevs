using System.Text;
using DesperateDevs.Networking;

namespace DesperateDevs.Logging.Appenders {

    public class TcpSocketAppender : AbstractTcpSocketAppender {

        protected override byte[] serializeMessage(Logger logger, LogLevel logLevel, string message) {
            return TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message));
        }
    }
}
