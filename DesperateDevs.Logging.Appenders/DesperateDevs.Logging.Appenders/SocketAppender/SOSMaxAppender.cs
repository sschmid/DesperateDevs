using System.Text;

namespace DesperateDevs.Logging.Appenders {

    public class SOSMaxAppender : AbstractSocketAppender {

        protected override byte[] serializeMessage(Logger logger, LogLevel logLevel, string message) {
            return Encoding.UTF8.GetBytes(formatLogMessage(logLevel.ToString(), message));
        }

        string formatLogMessage(string logLevel, string message) {
            var lines = message.Split('\n');
            var commandType = lines.Length == 1 ? "showMessage" : "showFoldMessage";
            var isMultiLine = lines.Length > 1;

            return string.Format("!SOS<{0} key=\"{1}\">{2}</{0}>\0",
                commandType,
                logLevel,
                isMultiLine ? multilineMessage(lines[0], message) : replaceXmlSymbols(message));
        }

        string multilineMessage(string title, string message) {
            return "<title>"
                + replaceXmlSymbols(title)
                + "</title><message>"
                + replaceXmlSymbols(message.Substring(message.IndexOf('\n') + 1))
                + "</message>";
        }

        string replaceXmlSymbols(string str) {
            return str.Replace("<", "&lt;")
                      .Replace(">", "&gt;")
                      .Replace("&lt;", "<![CDATA[<]]>")
                      .Replace("&gt;", "<![CDATA[>]]>")
                      .Replace("&", "<![CDATA[&]]>");
        }
    }
}
