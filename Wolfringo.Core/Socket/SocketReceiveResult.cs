using System;
using System.Net.WebSockets;

namespace TehGM.Wolfringo.Socket
{
    internal class SocketReceiveResult
    {
        public byte[] ContentBytes { get; }
        public WebSocketMessageType MessageType { get; }

        public SocketReceiveResult(byte[] contentBytes, int bytesRead, WebSocketMessageType messageType)
        {
            if (messageType == WebSocketMessageType.Close)
                throw new ArgumentException($"{messageType} message type not supported in {nameof(SocketReceiveResult)}", nameof(messageType));
            this.ContentBytes = new byte[bytesRead];
            Buffer.BlockCopy(contentBytes, 0, this.ContentBytes, 0, bytesRead);
            this.MessageType = messageType;
        }
    }
}
