using System.Text;
using DesperateDevs.Net;

namespace DesperateDevs.Logging.Appenders
{
    public class TcpSocketAppender : AbstractTcpSocketAppender
    {
        protected override byte[] SerializeMessage(Logger logger, LogLevel logLevel, string message) =>
            TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message));
    }
}
