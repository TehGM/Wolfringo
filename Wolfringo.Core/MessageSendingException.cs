using System;
using System.Net;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    [Serializable]
    public class MessageSendingException : Exception
    {
        private const string _defaultMessage = "Server responded with non-success status code";

        public WolfResponse Response { get; }
        public HttpStatusCode StatusCode => this.Response.ResponseCode;

        public MessageSendingException(WolfResponse response, string message, Exception innerException) 
            : base(message, innerException)
        {
            this.Response = response;
        }

        public MessageSendingException(WolfResponse response, Exception innerException)
            : this(response, _defaultMessage, innerException) { }

        public MessageSendingException(WolfResponse response, string message)
            : this(response, message, null) { }

        public MessageSendingException(WolfResponse response)
            : this(response, _defaultMessage) { }
    }
}
