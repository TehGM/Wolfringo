using System;
using System.Net;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    [Serializable]
    public class MessageSendingException : Exception
    {
        public IWolfResponse Response { get; }
        public HttpStatusCode StatusCode => this.Response.StatusCode;

        public MessageSendingException(IWolfResponse response, string message, Exception innerException) 
            : base(message, innerException)
        {
            this.Response = response;
        }

        public MessageSendingException(IWolfResponse response, Exception innerException)
            : this(response, BuildDefaultMessage(response), innerException) { }

        public MessageSendingException(IWolfResponse response, string message)
            : this(response, message, null) { }

        public MessageSendingException(IWolfResponse response)
            : this(response, BuildDefaultMessage(response)) { }

        private static string BuildDefaultMessage(IWolfResponse response)
            => $"Server responded with non-success status code: {(int)response.StatusCode} ({response.StatusCode})";
    }
}
