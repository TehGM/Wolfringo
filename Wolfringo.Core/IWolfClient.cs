using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    public interface IWolfClient
    {
        WolfUser CurrentUser { get; }

        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : WolfResponse;

        /// <summary>Rasied when the client connects to the server.</summary>
        event EventHandler Connected;
        /// <summary>Raised when the client disconnects from the server.</summary>
        event EventHandler Disconnected;
        event EventHandler<WolfMessageEventArgs> MessageReceived;
        event EventHandler<WolfMessageSentEventArgs> MessageSent;
        event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        void AddMessageListener<T>(Action<T> listener) where T : IWolfMessage;
        void RemoveMessageListener<T>(Action<T> listener) where T : IWolfMessage;
    }
}
