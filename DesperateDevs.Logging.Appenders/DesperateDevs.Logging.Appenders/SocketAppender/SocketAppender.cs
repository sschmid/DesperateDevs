using System.Text;

namespace DesperateDevs.Logging.Appenders {

    public class SocketAppender : AbstractSocketAppender {

        protected override byte[] serializeMessage(Logger logger, LogLevel logLevel, string message) {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
