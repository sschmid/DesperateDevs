using System;
using System.Linq;
using System.Text;
using DesperateDevs.Networking;
using NSpec;

class describe_TcpMessageParser : nspec {

    void when_parsing() {

        TcpMessageParser parser = null;

        before = () => { parser = new TcpMessageParser(); };

        it["wraps and unwraps message"] = () => {
            const string message = "123456";
            var bytes = Encoding.UTF8.GetBytes(message);
            var wrapped = TcpMessageParser.WrapMessage(bytes);
            var unwrapped = TcpMessageParser.UnwrapMessage(wrapped);
            Encoding.UTF8.GetString(unwrapped).should_be(message);
        };

        it["parses full message"] = () => {
            const string message = "123456";
            var messages = 0;
            parser.OnMessage += (p, b) => {
                messages += 1;
                p.should_be_same(parser);
                Encoding.UTF8.GetString(b).should_be(message);
            };

            var bytes = TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message));
            parser.Receive(bytes);
            messages.should_be(1);
        };

        it["doesn't parse partial message"] = () => {
            const string message = "123456";
            const string partialMessage = "123";

            parser.OnMessage += (p, b) => this.Fail();

            var lengthPrefix = BitConverter.GetBytes(message.Length);
            var prefixedMessage = new byte[lengthPrefix.Length + partialMessage.Length];
            Array.Copy(lengthPrefix, 0, prefixedMessage, 0, lengthPrefix.Length);

            var partialBytes = Encoding.UTF8.GetBytes(partialMessage);
            Array.Copy(partialBytes, 0, prefixedMessage, lengthPrefix.Length, partialBytes.Length);

            parser.Receive(prefixedMessage);
        };

        it["completes partial message"] = () => {
            const string message = "123456";
            const string partialMessage1 = "123";
            const string partialMessage2 = "456";

            var messages = 0;
            parser.OnMessage += (p, b) => {
                messages += 1;
                Encoding.UTF8.GetString(b).should_be(message);
            };

            var lengthPrefix = BitConverter.GetBytes(message.Length);
            var prefixedMessage = new byte[lengthPrefix.Length + partialMessage1.Length];
            Array.Copy(lengthPrefix, 0, prefixedMessage, 0, lengthPrefix.Length);

            var partialBytes1 = Encoding.UTF8.GetBytes(partialMessage1);
            Array.Copy(partialBytes1, 0, prefixedMessage, lengthPrefix.Length, partialBytes1.Length);

            var partialBytes2 = Encoding.UTF8.GetBytes(partialMessage2);

            parser.Receive(prefixedMessage);
            parser.Receive(partialBytes2);

            messages.should_be(1);
        };

        it["can read multiple messages"] = () => {
            const string message1 = "123456";
            const string message2 = "abcdef";
            var messages = 0;
            parser.OnMessage += (p, b) => {
                messages += 1;

                if (messages == 1) {
                    Encoding.UTF8.GetString(b).should_be(message1);
                } else if (messages == 2) {
                    Encoding.UTF8.GetString(b).should_be(message2);
                }
            };

            var bytes1 = TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message1));
            var bytes2 = TcpMessageParser.WrapMessage(Encoding.UTF8.GetBytes(message2));

            parser.Receive(bytes1.Concat(bytes2).ToArray());
            messages.should_be(2);
        };
    }
}
