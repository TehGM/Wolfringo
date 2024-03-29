﻿using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Socket;

namespace TehGM.Wolfringo.Caching
{
    /// <summary>Utility grouping common entity caches together.</summary>
    /// <remarks><para>This utility contains caches for entities that Wolf client is caching.</para>
    /// <para>Each cache can be separately enabled or disabled.</para></remarks>
    public interface IWolfClientCache : IWolfClientCacheAccessor
    {
        /// <summary>Handle message sent by the client, and the server's response.</summary>
        /// <param name="client">WOLF client that sent the message.</param>
        /// <param name="message">Sent message.</param>
        /// <param name="response">Response received.</param>
        /// <param name="rawResponse">Raw response data.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        Task OnMessageSentAsync(IWolfClient client, IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default);
        /// <summary>Handle message received from the server.</summary>
        /// <param name="client">WOLF client that received the message.</param>
        /// <param name="message">Received message.</param>
        /// <param name="rawMessage">Raw received message.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        Task OnMessageReceivedAsync(IWolfClient client, IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default);
        /// <summary>Invoked when client is about to connect to the server.</summary>
        /// <param name="client">WOLF client that is connecting to the server.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        Task OnConnectingAsync(IWolfClient client, CancellationToken cancellationToken = default);
        /// <summary>Invoked when the client has disconnected from the server.</summary>
        /// <param name="client">WOLF client that has disconnected from the server.</param>
        /// <param name="e">Event args raised when connection was closed.</param>
        void OnDisconnected(IWolfClient client, SocketClosedEventArgs e);
    }
}
