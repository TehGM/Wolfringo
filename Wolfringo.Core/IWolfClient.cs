using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>A Wolf client.</summary>
    public interface IWolfClient
    {
        /// <summary>Currently logged in user. Null if not connected.</summary>
        uint? CurrentUserID { get; }

        /// <summary>Connects to the server.</summary>
        /// <param name="cancellationToken">Token to abort entire connection.</param>
        Task ConnectAsync(CancellationToken cancellationToken = default);
        /// <summary>Disconnects from the server.</summary>
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        /// <summary>Sends message, and waits for response from the server.</summary>
        /// <remarks><para>If client uses <see cref="ResponseTypeResolver"/>, the type of response provided with 
        /// <see cref="ResponseTypeAttribute"/> on <paramref name="message"/> will be used for deserialization, 
        /// and <typeparamref name="TResponse"/> will be used only for casting. If <see cref="ResponseTypeAttribute"/> is not set on
        /// <paramref name="message"/>, <typeparamref name="TResponse"/> will be used for deserialization as normal.</para></remarks>
        /// <typeparam name="TResponse">Response type to use for casting of response.</typeparam>
        /// <param name="message">Message to send.</param>
        /// <returns>Sending response.</returns>
        /// <exception cref="MessageSendingException">Server responded with error.</exception>
        Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : IWolfResponse;

        /// <summary>Rasied when the client connects to the server.</summary>
        event EventHandler Connected;
        /// <summary>Raised when the client disconnects from the server.</summary>
        event EventHandler Disconnected;
        /// <summary>Raised when the client receives an event from the server.</summary>
        event EventHandler<WolfMessageEventArgs> MessageReceived;
        /// <summary>Raised when the client sends a message to the server.</summary>
        event EventHandler<WolfMessageSentEventArgs> MessageSent;
        /// <summary>Raised when an error has occured.</summary>
        event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        /// <summary>Adds event listener.</summary>
        /// <param name="listener">Callback to invoke on event.</param>
        void AddMessageListener(IMessageCallback listener);
        /// <summary>Removes event listener.</summary>
        /// <param name="listener">Callback to remove.</param>
        void RemoveMessageListener(IMessageCallback listener);
    }
}
