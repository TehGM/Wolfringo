using System;
using System.Net;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    [Serializable]
    public class MessageSendingException : Exception
    {
        private const string _defaultMessage = "Server responded with non-success status code";

        public IWolfResponse Response { get; }
        public HttpStatusCode StatusCode => this.Response.ResponseCode;

        public MessageSendingException(IWolfResponse response, string message, Exception innerException) 
            : base(message, innerException)
        {
            this.Response = response;
        }

        public MessageSendingException(IWolfResponse response, Exception innerException)
            : this(response, _defaultMessage, innerException) { }

        public MessageSendingException(IWolfResponse response, string message)
            : this(response, message, null) { }

        public MessageSendingException(IWolfResponse response)
            : this(response, _defaultMessage) { }
    }
}
