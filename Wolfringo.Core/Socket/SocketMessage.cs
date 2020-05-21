using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace TehGM.Wolfringo.Socket
{
    public class SocketMessage
    {
        /// <summary>Type of SocketIO message.</summary>
        public SocketMessageType Type { get; }
        /// <summary>ID of the message in case of events.</summary>
        public uint? ID { get; }
        /// <summary>Raw JSON payload.</summary>
        public JToken Payload { get; }
        /// <summary>Count of binary messages that will be sent after this message.</summary>
        public int BinaryMessagesCount { get; }

        private string _rawMessage;

        public SocketMessage(SocketMessageType type, uint? id, JToken payload, int binaryCount = 0)
        {
            this.Type = type;
            this.ID = id;
            this.Payload = payload;
            this.BinaryMessagesCount = binaryCount;
        }

        /// <summary>Parses raw message text from stream.</summary>
        /// <param name="rawMessage">Message text.</param>
        /// <returns>Parsed message</returns>
        public static SocketMessage Parse(string rawMessage)
        {
            if (rawMessage == null)
                throw new ArgumentNullException(nameof(rawMessage));
            if (string.IsNullOrWhiteSpace(rawMessage))
                throw new ArgumentException("Raw message cannot be empty", nameof(rawMessage));

            // get msg type
            SocketMessageType msgType = ParseMessageType(rawMessage, out int parserIndex);
            // get payload
            JToken payload = ParsePayload(rawMessage, out int payloadIndex);
            // get binary count
            int binaryCount = ParseBinaryCount(rawMessage, ref parserIndex, payloadIndex);
            // get message id
            uint? id = ParseMessageID(rawMessage, ref parserIndex, payloadIndex);

            // return results
            SocketMessage result = new SocketMessage(msgType, id, payload, binaryCount);
            result._rawMessage = rawMessage;
            return result;
        }

        private static SocketMessageType ParseMessageType(string rawMessage, out int length)
        {
            if (rawMessage[0] == '4')
                length = 2;
            else length = 1;
            int msgType = int.Parse(rawMessage.Substring(0, length));
            return (SocketMessageType)msgType;
        }

        private static int ParseBinaryCount(string rawMessage, ref int parserIndex, int payloadIndex = -1)
        {
            int searchCount = payloadIndex > 0 ? payloadIndex - parserIndex : rawMessage.Length - parserIndex;
            // ignore tacks in the payload
            int tackIndex = rawMessage.IndexOf('-', parserIndex, searchCount);
            if (tackIndex < 0)
                return 0;
            int result = int.Parse(rawMessage.Substring(parserIndex, tackIndex - parserIndex));
            parserIndex = tackIndex + 1;
            return result;
        }

        private static uint? ParseMessageID(string rawMessage, ref int parserIndex, int payloadIndex = -1)
        {
            // if message has -, the ID will be after it
            int tackIndex = payloadIndex > parserIndex
                ? rawMessage.IndexOf('-', parserIndex, payloadIndex - parserIndex)
                : rawMessage.IndexOf('-', parserIndex);
            if (tackIndex >= 0)
                parserIndex = tackIndex;

            // if there's payload, id length is from index to payload, otherwise to end of the message
            int idLength = (payloadIndex > parserIndex ? payloadIndex : rawMessage.Length) - parserIndex;

            if (uint.TryParse(rawMessage.Substring(parserIndex, idLength), out uint result))
                return result;
            return null;
        }

        private static JToken ParsePayload(string rawMessage, out int payloadIndex)
        {
            payloadIndex = -1;
            int arrayPayloadIndex = rawMessage.IndexOf('[');
            int objPayloadIndex = rawMessage.IndexOf('{');

            if (arrayPayloadIndex >= 0)
                payloadIndex = arrayPayloadIndex;
            if (objPayloadIndex >= 0 && objPayloadIndex < arrayPayloadIndex)
                payloadIndex = objPayloadIndex;
            if (payloadIndex < 0)
                return null;
            return JToken.Parse(rawMessage.Substring(payloadIndex));
        }

        public override string ToString()
        {
            if (_rawMessage == null)
            {
                // if message is null, for example when not initialized by Parse method, build it first
                StringBuilder builder = new StringBuilder();
                builder.Append((int)this.Type);
                if (this.BinaryMessagesCount > 0)
                {
                    builder.Append(this.BinaryMessagesCount);
                    builder.Append('-');
                }
                if (this.ID != null)
                    builder.Append(this.ID.Value);
                if (this.Payload != null)
                    builder.Append(this.Payload.ToString(Newtonsoft.Json.Formatting.None));
                _rawMessage = builder.ToString();
            }
            return _rawMessage;
        }
    }

    /// <summary>Represents type of SocketIO message.</summary>
    public enum SocketMessageType
    {
        SID = 0,
        TransportClose = 1,
        Ping = 2,
        Pong = 3,
        Connect = 40,
        Disconnect = 41,
        Event = 42,
        EventAck = 43,
        Error = 44,
        BinaryEvent = 45,
        BinaryEventAck = 46
    }
}
