using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Net.Tests
{
    public class TcpMessageParserTests
    {
        readonly TcpMessageParser _parser = new TcpMessageParser();

        [Fact]
        public void WrapsAndUnwrapsMessage()
        {
            const string message = "123456";
            var bytes = Encoding.UTF8.GetBytes(message);
            var wrapped = TcpMessageParser.WrapMessage(bytes);
            var unwrapped = TcpMessageParser.UnwrapMessage(wrapped);
            Encoding.UTF8.GetString(unwrapped).Should().Be(message);
        }

        [Fact]
        public void ParsesFullMessage()
        {
            const string message = "123456";
            var messages = 0;
            _parser.OnMessage += (p, b) =>
            {
                messages += 1;
                p.Should().BeSameAs(_parser);
                Encoding.UTF8.GetString(b).Should().Be(message);
            };

            var bytes = TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message));
            _parser.Receive(bytes);
            messages.Should().Be(1);
        }

        [Fact]
        public void DoesNotParsePartialMessage()
        {
            const string message = "123456";
            const string partialMessage = "123";

            _parser.OnMessage += (p, b) => throw new Exception("parser.OnMessage");

            var lengthPrefix = BitConverter.GetBytes(message.Length);
            var prefixedMessage = new byte[lengthPrefix.Length + partialMessage.Length];
            Array.Copy(lengthPrefix, 0, prefixedMessage, 0, lengthPrefix.Length);

            var partialBytes = Encoding.UTF8.GetBytes(partialMessage);
            Array.Copy(partialBytes, 0, prefixedMessage, lengthPrefix.Length, partialBytes.Length);

            _parser.Receive(prefixedMessage);
        }

        [Fact]
        public void CompletesPartialMessage()
        {
            const string message = "123456";
            const string partialMessage1 = "123";
            const string partialMessage2 = "456";

            var messages = 0;
            _parser.OnMessage += (p, b) =>
            {
                messages += 1;
                Encoding.UTF8.GetString(b).Should().Be(message);
            };

            var lengthPrefix = BitConverter.GetBytes(message.Length);
            var prefixedMessage = new byte[lengthPrefix.Length + partialMessage1.Length];
            Array.Copy(lengthPrefix, 0, prefixedMessage, 0, lengthPrefix.Length);

            var partialBytes1 = Encoding.UTF8.GetBytes(partialMessage1);
            Array.Copy(partialBytes1, 0, prefixedMessage, lengthPrefix.Length, partialBytes1.Length);

            var partialBytes2 = Encoding.UTF8.GetBytes(partialMessage2);

            _parser.Receive(prefixedMessage);
            _parser.Receive(partialBytes2);

            messages.Should().Be(1);
        }

        [Fact]
        public void ReadsMultipleMessages()
        {
            const string message1 = "123456";
            const string message2 = "abcdef";
            var messages = 0;
            _parser.OnMessage += (p, b) =>
            {
                messages += 1;

                if (messages == 1)
                {
                    Encoding.UTF8.GetString(b).Should().Be(message1);
                }
                else if (messages == 2)
                {
                    Encoding.UTF8.GetString(b).Should().Be(message2);
                }
            };

            var bytes1 = TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message1));
            var bytes2 = TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message2));

            _parser.Receive(bytes1.Concat(bytes2).ToArray());
            messages.Should().Be(2);
        }
    }
}
