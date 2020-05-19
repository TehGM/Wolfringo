using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Socket
{
    [Serializable]
    public class SocketMessageEventArgs : EventArgs
    {
        public SocketMessage Message { get; }
        public IEnumerable<byte[]> BinaryMessages { get; }

        public SocketMessageEventArgs(SocketMessage message, IEnumerable<byte[]> binaryMessages)
        {
            this.Message = message;
            this.BinaryMessages = binaryMessages;
        }

        public SocketMessageEventArgs(SocketMessage message)
            : this(message, Enumerable.Empty<byte[]>()) { }
    }
}
