using System;
using System.Net.WebSockets;

namespace TehGM.Wolfringo.Socket
{
    /// <summary>Arguments for <see cref="ISocketClient.Disconnected"/> event.</summary>
    [Serializable]
    public class SocketClosedEventArgs : EventArgs
    {
        /// <summary>Indicates the reason why the remote endpoint initiated the close handshake.</summary>
        public WebSocketCloseStatus CloseStatus { get; }
        /// <summary>The optional description that describes why the close handshake has been initiated by the remote endpoint.</summary>
        public string CloseMessage { get; }
        /// <summary>Exception that caused socket closing.</summary>
        public Exception Exception { get; }

        /// <summary>Creates new event args instance.</summary>
        /// <param name="closeStatus">Indicates the reason why the remote endpoint initiated the close handshake.</param>
        /// <param name="closeMessage">The optional description that describes why the close handshake has been initiated by the remote endpoint.</param>
        /// <param name="exception">Exception that caused socket closing.</param>
        public SocketClosedEventArgs(WebSocketCloseStatus closeStatus, string closeMessage, Exception exception) : base()
        {
            this.CloseStatus = closeStatus;
            this.CloseMessage = closeMessage;
            this.Exception = exception;
        }

        /// <summary>Creates new event args instance.</summary>
        /// <param name="closeStatus">Indicates the reason why the remote endpoint initiated the close handshake.</param>
        /// <param name="closeMessage">The optional description that describes why the close handshake has been initiated by the remote endpoint.</param>
        public SocketClosedEventArgs(WebSocketCloseStatus closeStatus, string closeMessage)
            : this(closeStatus, closeMessage, null) { }
    }
}
