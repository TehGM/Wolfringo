using System;
using System.Net.WebSockets;

namespace TehGM.Wolfringo.Socket
{
    internal class SocketReceiveResult
    {
        public byte[] ContentBytes { get; }
        public int BytesRead { get; }
        public WebSocketMessageType MessageType { get; }

        public SocketReceiveResult(byte[] contentBytes, int bytesRead, WebSocketMessageType messageType)
        {
            if (messageType == WebSocketMessageType.Close)
                throw new ArgumentException($"{messageType} message type not supported in {nameof(SocketReceiveResult)}", nameof(messageType));
            this.ContentBytes = contentBytes;
            this.BytesRead = bytesRead;
            this.MessageType = messageType;
        }
    }
}
