using FluentAssertions;
using Xunit;

namespace DesperateDevs.Logging.Formatters.Tests
{
    public class DefaultLogMessageFormatterTests
    {
        readonly DefaultLogMessageFormatter _formatter;
        readonly Logger _logger;

        public DefaultLogMessageFormatterTests()
        {
            _formatter = new DefaultLogMessageFormatter();
            _logger = new Logger("MyLogger");
        }

        [Fact]
        public void FormatsString()
        {
            var message = _formatter.FormatMessage(_logger, LogLevel.Debug, "test");
            message.Should().Be("[DEBUG] MyLogger: test");
        }

        [Fact]
        public void PadsLogLevel()
        {
            var message1 = _formatter.FormatMessage(_logger, LogLevel.Debug, "test1");
            var message2 = _formatter.FormatMessage(_logger, LogLevel.Warn, "test2");
            message1.Should().Be("[DEBUG] MyLogger: test1");
            message2.Should().Be("[WARN]  MyLogger: test2");
        }
    }
}
