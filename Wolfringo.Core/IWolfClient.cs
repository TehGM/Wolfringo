using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    public interface IWolfClient
    {
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

        void AddMessageListener(IMessageCallback listener);
        void RemoveMessageListener(IMessageCallback listener);

        Task<IEnumerable<WolfUser>> GetUsersAsync(IEnumerable<uint> userIDs, CancellationToken cancellationToken = default);
        Task<WolfUser> GetCurrentUserAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<WolfGroup>> GetGroupsAsync(IEnumerable<uint> groupIDs, CancellationToken cancellationToken = default);
        //Task<IEnumerable<WolfCharm>> GetCharmsAsync(IEnumerable<uint> charmIDs, CancellationToken cancellationToken = default);
    }
}
