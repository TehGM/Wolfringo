using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Utilities.Interactive;

namespace TehGM.Wolfringo
{
    /// <summary>Set of helper methods to make work with <see cref="IWolfClient"/> combined with <see cref="IInteractiveListener{T}"/> easier.</summary>
    public static class InteractiveExtensions
    {
        #region CancellationToken versions
        /// <summary>Awaits next message of given type that matches specified conditions.</summary>
        /// <param name="client">Client to await message from.</param>
        /// <param name="conditions">Conditions that received message needs to match.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextInGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextGroupByUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextPrivateByUserAsync(IWolfClient, uint, CancellationToken)"/>
        public static Task<T> AwaitNextAsync<T>(this IWolfClient client, Func<T, bool> conditions, 
            CancellationToken cancellationToken = default) where T : IWolfMessage
        {
            IInteractiveListener<T> listener = new InteractiveListener<T>(conditions);
            return listener.AwaitNextAsync(client, cancellationToken);
        }

        /// <summary>Awaits next chat message in PM from specified user.</summary>
        /// <param name="client">Client to await message from.</param>
        /// <param name="userID">User ID to await the message from.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextInGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextGroupByUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextAsync{T}(IWolfClient, Func{T, bool}, CancellationToken)"/>
        public static Task<ChatMessage> AwaitNextPrivateByUserAsync(this IWolfClient client, uint userID, 
            CancellationToken cancellationToken = default)
            => AwaitNextAsync<ChatMessage>(client, 
                message => 
                     message.IsPrivateMessage && message.SenderID == userID, 
                cancellationToken);

        /// <summary>Awaits next chat message in specified group from specified user.</summary>
        /// <param name="client">Client to await message from.</param>
        /// <param name="userID">User ID to await the message from.</param>
        /// <param name="groupID">Group ID to await the message from.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextInGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextPrivateByUserAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextAsync{T}(IWolfClient, Func{T, bool}, CancellationToken)"/>
        public static Task<ChatMessage> AwaitNextGroupByUserAsync(this IWolfClient client, uint userID, uint groupID,
            CancellationToken cancellationToken = default)
            => AwaitNextAsync<ChatMessage>(client,
                message => 
                    message.IsGroupMessage && message.RecipientID == groupID && message.SenderID == userID,
                cancellationToken);

        /// <summary>Awaits next chat message in specified group.</summary>
        /// <param name="client">Client to await message from.</param>
        /// <param name="groupID">Group ID to await the message from.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextPrivateByUserAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextGroupByUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="AwaitNextAsync{T}(IWolfClient, Func{T, bool}, CancellationToken)"/>
        public static Task<ChatMessage> AwaitNextInGroupAsync(this IWolfClient client, uint groupID,
            CancellationToken cancellationToken = default)
            => AwaitNextAsync<ChatMessage>(client,
                message =>
                    message.IsGroupMessage && message.RecipientID == groupID,
                cancellationToken);
        #endregion

        #region Timeout versions
        /// <summary>Awaits next message of given type that matches specified conditions.</summary>
        /// <remarks><para>This method actually uses a <see cref="CancellationTokenSource"/> under the hood, and simply cancels the token
        /// after specified <paramref name="timeout"/>.</para>
        /// <para>If the task is cancelled a default (most likely null> value will be returned. 
        /// <see cref="TaskCanceledException"/> will not be thrown.</para></remarks>
        /// <param name="client">Client to await message from.</param>
        /// <param name="conditions">Conditions that received message needs to match.</param>
        /// <param name="timeout">Timeout after which waiting will be aborted.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextGroupByUserAsync(IWolfClient, uint, uint, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextInGroupAsync(IWolfClient, uint, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextPrivateByUserAsync(IWolfClient, uint, TimeSpan, CancellationToken)"/>
        public static async Task<T> AwaitNextAsync<T>(this IWolfClient client, Func<T, bool> conditions,
            TimeSpan timeout, CancellationToken cancellationToken = default) where T : IWolfMessage
        {
            try
            {
                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    Task<T> task = AwaitNextAsync(client, conditions, cts.Token);
                    cts.CancelAfter(timeout);
                    return await task.ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
                return default;
            }
        }

        /// <summary>Awaits next chat message in PM from specified user.</summary>
        /// <remarks><para>This method actually uses a <see cref="CancellationTokenSource"/> under the hood, and simply cancels the token
        /// after specified <paramref name="timeout"/>.</para>
        /// <para>If the task is cancelled a default (most likely null> value will be returned. 
        /// <see cref="TaskCanceledException"/> will not be thrown.</para></remarks>
        /// <param name="client">Client to await message from.</param>
        /// <param name="userID">User ID to await the message from.</param>
        /// <param name="timeout">Timeout after which waiting will be aborted.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextAsync{T}(IWolfClient, Func{T, bool}, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextGroupByUserAsync(IWolfClient, uint, uint, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextInGroupAsync(IWolfClient, uint, TimeSpan, CancellationToken)"/>
        public static Task<ChatMessage> AwaitNextPrivateByUserAsync(this IWolfClient client, uint userID,
            TimeSpan timeout, CancellationToken cancellationToken = default)
            => AwaitNextAsync<ChatMessage>(client,
                message =>
                     message.IsPrivateMessage && message.SenderID == userID,
                timeout, cancellationToken);

        /// <summary>Awaits next chat message in specified group from specified user.</summary>
        /// <remarks><para>This method actually uses a <see cref="CancellationTokenSource"/> under the hood, and simply cancels the token
        /// after specified <paramref name="timeout"/>.</para>
        /// <para>If the task is cancelled a default (most likely null> value will be returned. 
        /// <see cref="TaskCanceledException"/> will not be thrown.</para></remarks>
        /// <param name="client">Client to await message from.</param>
        /// <param name="userID">User ID to await the message from.</param>
        /// <param name="groupID">Group ID to await the message from.</param>
        /// <param name="timeout">Timeout after which waiting will be aborted.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextAsync{T}(IWolfClient, Func{T, bool}, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextInGroupAsync(IWolfClient, uint, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextPrivateByUserAsync(IWolfClient, uint, TimeSpan, CancellationToken)"/>
        public static Task<ChatMessage> AwaitNextGroupByUserAsync(this IWolfClient client, uint userID, uint groupID,
            TimeSpan timeout, CancellationToken cancellationToken = default)
            => AwaitNextAsync<ChatMessage>(client,
                message =>
                    message.IsGroupMessage && message.RecipientID == groupID && message.SenderID == userID,
                timeout, cancellationToken);

        /// <summary>Awaits next chat message in specified group.</summary>
        /// <remarks><para>This method actually uses a <see cref="CancellationTokenSource"/> under the hood, and simply cancels the token
        /// after specified <paramref name="timeout"/>.</para>
        /// <para>If the task is cancelled a default (most likely null> value will be returned. 
        /// <see cref="TaskCanceledException"/> will not be thrown.</para></remarks>
        /// <param name="client">Client to await message from.</param>
        /// <param name="groupID">Group ID to await the message from.</param>
        /// <param name="timeout">Timeout after which waiting will be aborted.</param>
        /// <returns>Next message.</returns>
        /// <seealso cref="AwaitNextAsync{T}(IWolfClient, Func{T, bool}, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextGroupByUserAsync(IWolfClient, uint, uint, TimeSpan, CancellationToken)"/>
        /// <seealso cref="AwaitNextPrivateByUserAsync(IWolfClient, uint, TimeSpan, CancellationToken)"/>
        public static Task<ChatMessage> AwaitNextInGroupAsync(this IWolfClient client, uint groupID,
            TimeSpan timeout, CancellationToken cancellationToken = default)
            => AwaitNextAsync<ChatMessage>(client,
                message =>
                    message.IsGroupMessage && message.RecipientID == groupID,
                timeout, cancellationToken);
        #endregion
    }
}
