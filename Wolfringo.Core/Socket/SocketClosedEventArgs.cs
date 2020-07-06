using System;
using System.Net.WebSockets;

namespace TehGM.Wolfringo.Socket
{
    [Serializable]
    public class SocketClosedEventArgs : EventArgs
    {
        /// <summary>Indicates the reason why the remote endpoint initiated the close handshake.</summary>
        public WebSocketCloseStatus CloseStatus { get; }
        /// <summary>Returns the optional description that describes why the close handshake has been initiated by the remote endpoint.</summary>
        public string CloseMessage { get; }
        /// <summary>Exception that caused socket closing.</summary>
        public Exception Exception { get; }

        public SocketClosedEventArgs(WebSocketCloseStatus closeStatus, string closeMessage, Exception exception) : base()
        {
            this.CloseStatus = closeStatus;
            this.CloseMessage = closeMessage;
            this.Exception = exception;
        }

        public SocketClosedEventArgs(WebSocketCloseStatus closeStatus, string closeMessage)
            : this(closeStatus, closeMessage, null) { }
    }
}
