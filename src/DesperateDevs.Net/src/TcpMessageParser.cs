using System;

namespace DesperateDevs.Net
{
    public class TcpMessageParser
    {
        public const int BufferLength = sizeof(int);

        public delegate void TcpMessageParserMessage(TcpMessageParser messageParser, byte[] bytes);

        public event TcpMessageParserMessage OnMessage;

        readonly byte[] _lengthBuffer = new byte[BufferLength];

        byte[] _messageBuffer;
        int _readIndex;
        int _writeIndex;

        public static byte[] WrapMessage(byte[] message)
        {
            var lengthPrefix = BitConverter.GetBytes(message.Length);
            var prefixedMessage = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(prefixedMessage, 0);
            message.CopyTo(prefixedMessage, lengthPrefix.Length);
            return prefixedMessage;
        }

        public static byte[] UnwrapMessage(byte[] message)
        {
            var lengthPrefix = new byte[BufferLength];
            var unwrappedMessage = new byte[message.Length - BufferLength];
            Array.Copy(message, lengthPrefix, BufferLength);
            Array.Copy(message, BufferLength, unwrappedMessage, 0, unwrappedMessage.Length);
            return unwrappedMessage;
        }

        public void Receive(byte[] bytes)
        {
            while (_readIndex < bytes.Length)
            {
                if (_messageBuffer == null)
                {
                    ReadLength(bytes);
                }

                if (_messageBuffer != null)
                {
                    ReadMessage(bytes);
                }
            }

            _readIndex = 0;
        }

        void ReadLength(byte[] bytes)
        {
            if (Read(bytes, _lengthBuffer, BufferLength))
            {
                _messageBuffer = new byte[BitConverter.ToInt32(_lengthBuffer, 0)];
            }
        }

        void ReadMessage(byte[] bytes)
        {
            if (Read(bytes, _messageBuffer, _messageBuffer.Length))
            {
                var message = _messageBuffer;
                _messageBuffer = null;

                if (OnMessage != null)
                {
                    OnMessage(this, message);
                }
            }
        }

        bool Read(byte[] bytes, byte[] buffer, int length)
        {
            var bytesToReadToFillBuffer = length - _writeIndex;
            var availableBytesToRead = bytes.Length - _readIndex;
            if (bytesToReadToFillBuffer <= availableBytesToRead)
            {
                Array.Copy(bytes, _readIndex, buffer, _writeIndex, bytesToReadToFillBuffer);
                _readIndex += bytesToReadToFillBuffer;
                _writeIndex = 0;
                return true;
            }

            Array.Copy(bytes, _readIndex, buffer, _writeIndex, availableBytesToRead);
            _readIndex += availableBytesToRead;
            _writeIndex += availableBytesToRead;
            return false;
        }
    }
}
