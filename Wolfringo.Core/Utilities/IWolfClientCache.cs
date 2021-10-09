﻿using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Utility grouping common entity caches together.</summary>
    /// <remarks><para>This utility contains caches for entities that Wolf client is caching.</para>
    /// <para>Each cache can be separately enabled or disabled.</para></remarks>
    public interface IWolfClientCache
    {
        /// <summary>Users cache.</summary>
        IWolfEntityCache<WolfUser> UsersCache { get; }
        /// <summary>Groups cache.</summary>
        IWolfEntityCache<WolfGroup> GroupsCache { get; }
        /// <summary>Charms cache.</summary>
        IWolfEntityCache<WolfCharm> CharmsCache { get; }
        /// <summary>Achievements cache.</summary>
        IWolfEntityCache<WolfLanguage, WolfAchievement> AchievementsCache { get; }

        /// <summary>Handle message sent by the client, and the server's response.</summary>
        /// <param name="client">WOLF client that sent the message.</param>
        /// <param name="message">Sent message.</param>
        /// <param name="response">Response received.</param>
        /// <param name="rawResponse">Raw response data.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        Task HandleMessageSentAsync(IWolfClient client, IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default);
        /// <summary>Handle message received from the server.</summary>
        /// <param name="client">WOLF client that received the message.</param>
        /// <param name="message">Received message.</param>
        /// <param name="rawMessage">Raw received message.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        Task HandleMessageReceivedAsync(IWolfClient client, IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default);
        /// <summary>Clear all caches.</summary>
        void ClearAll();
    }
}