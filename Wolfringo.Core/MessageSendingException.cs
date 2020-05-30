using System;
using System.Net;
using System.Text;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    [Serializable]
    public class MessageSendingException : Exception
    {
        public IWolfMessage SentMessage { get; }
        public IWolfResponse Response { get; }
        public HttpStatusCode StatusCode => this.Response.StatusCode;

        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response, string message, Exception innerException) 
            : base(message, innerException)
        {
            this.SentMessage = sentMessage;
            this.Response = response;
        }

        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response, Exception innerException)
            : this(sentMessage, response, BuildDefaultMessage(sentMessage.Command, response), innerException) { }

        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response, string message)
            : this(sentMessage, response, message, null) { }

        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response)
            : this(sentMessage, response, BuildDefaultMessage(sentMessage.Command, response), null) { }

        private static string BuildDefaultMessage(string sentCommand, IWolfResponse response)
        {
            StringBuilder builder = new StringBuilder($"Server responded with non-success status code: {(int)response.StatusCode} ({response.StatusCode})");
            if (response is WolfResponse wolfResponse && wolfResponse.ErrorCode != null)
            {
                builder.AppendLine();
                builder.Append("Error ");
                builder.Append((int)wolfResponse.ErrorCode);
                builder.Append(": ");
                builder.Append(wolfResponse.ErrorCode.Value.GetDescription(sentCommand));

                if (!string.IsNullOrWhiteSpace(wolfResponse.ErrorMessage))
                {
                    builder.Append(" - ");
                    builder.Append(wolfResponse.ErrorMessage);
                }
            }
            return builder.ToString();
        }
    }
}
