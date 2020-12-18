using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Socket
{
    /// <summary>Event args for <see cref="ISocketClient.MessageReceived"/> and <see cref="ISocketClient.MessageSent"/>.</summary>
    [Serializable]
    public class SocketMessageEventArgs : EventArgs
    {
        /// <summary>The text message.</summary>
        public SocketMessage Message { get; }
        /// <summary>Binary messages associated with this message. Might be null.</summary>
        public IEnumerable<byte[]> BinaryMessages { get; }

        /// <summary>Creates a new event args instance.</summary>
        /// <param name="message">Socket message that was received/sent.</param>
        /// <param name="binaryMessages">Binary messages that were received/sent.</param>
        public SocketMessageEventArgs(SocketMessage message, IEnumerable<byte[]> binaryMessages) : base()
        {
            this.Message = message;
            this.BinaryMessages = binaryMessages;
        }

        /// <summary>Creates a new event args instance.</summary>
        /// <param name="message">Socket message that was received/sent.</param>
        public SocketMessageEventArgs(SocketMessage message)
            : this(message, Enumerable.Empty<byte[]>()) { }
    }
}
