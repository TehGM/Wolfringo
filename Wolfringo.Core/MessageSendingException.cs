using System;
using System.Net;
using System.Text;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    /// <summary>An exception thrown when server responds with an error response.</summary>
    [Serializable]
    public class MessageSendingException : Exception
    {
        /// <summary>Message sent to the server.</summary>
        public IWolfMessage SentMessage { get; }
        /// <summary>Server's response.</summary>
        public IWolfResponse Response { get; }
        /// <summary>Error code of the response.</summary>
        public HttpStatusCode StatusCode => this.Response.StatusCode;

        /// <summary>Creates a new instance of the exception.</summary>
        /// <param name="sentMessage">Message sent to the server.</param>
        /// <param name="response">Server's response.</param>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response, string message, Exception innerException) 
            : base(message, innerException)
        {
            this.SentMessage = sentMessage;
            this.Response = response;
        }

        /// <summary>Creates a new instance of the exception.</summary>
        /// <param name="sentMessage">Message sent to the server.</param>
        /// <param name="response">Server's response.</param>
        /// <param name="innerException">Inner exception.</param>
        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response, Exception innerException)
            : this(sentMessage, response, BuildDefaultMessage(sentMessage.Command, response), innerException) { }

        /// <summary>Creates a new instance of the exception.</summary>
        /// <param name="sentMessage">Message sent to the server.</param>
        /// <param name="response">Server's response.</param>
        /// <param name="message">Exception message.</param>
        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response, string message)
            : this(sentMessage, response, message, null) { }

        /// <summary>Creates a new instance of the exception.</summary>
        /// <param name="sentMessage">Message sent to the server.</param>
        /// <param name="response">Server's response.</param>
        public MessageSendingException(IWolfMessage sentMessage, IWolfResponse response)
            : this(sentMessage, response, BuildDefaultMessage(sentMessage.Command, response), null) { }

        /// <summary>Builds exception message based on sent command and server's response.</summary>
        /// <param name="sentCommand">Command sent to the server.</param>
        /// <param name="response">Server's response.</param>
        /// <returns>Build exception message.</returns>
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
