using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands
{
    public static class CommandContextExtensions
    {
        /// <summary>Gets profile of the user that sent the command.</summary>
        /// <param name="cancellationToken">Token to cancel server request with.</param>
        /// <returns>Profile of the user that sent the command</returns>
        /// <seealso cref="GetRecipientAsync{T}(CancellationToken)"/>
        public static Task<WolfUser> GetSenderAsync(this ICommandContext context, CancellationToken cancellationToken = default)
            => GetUserAsync(context, context.Message.SenderID.Value, cancellationToken);

        /// <summary>Gets profile of the recipient of the message.</summary>
        /// <typeparam name="T">Type of recipient, for example <see cref="WolfUser"/> or <see cref="WolfGroup"/>.</typeparam>
        /// <param name="cancellationToken">Token to cancel server request with.</param>
        /// <returns>Profile of the message's recipient; if <typeparamref name="T"/> does not match the message type, null will be returned.</returns>
        /// <seealso cref="GetSenderAsync(CancellationToken)"/>
        public static async Task<T> GetRecipientAsync<T>(this ICommandContext context, CancellationToken cancellationToken = default) where T : class, IWolfEntity
        {
            if (context.Message.IsGroupMessage)
            {
                if (context.Client is IWolfClientCacheAccessor cache)
                {
                    WolfGroup result = cache.GetCachedGroup(context.Message.RecipientID);
                    if (result != null)
                        return result as T;
                }

                GroupProfileResponse response = await context.Client.SendAsync<GroupProfileResponse>(
                    new GroupProfileMessage(new uint[] { context.Message.RecipientID }, true), cancellationToken).ConfigureAwait(false);
                return response?.GroupProfiles?.FirstOrDefault(g => g.ID == context.Message.RecipientID) as T;
            }
            else
                return await GetUserAsync(context, context.Message.RecipientID, cancellationToken) as T;
        }

        private static async Task<WolfUser> GetUserAsync(ICommandContext context, uint id, CancellationToken cancellationToken = default)
        {
            if (context.Client is IWolfClientCacheAccessor cache)
            {
                WolfUser result = cache.GetCachedUser(id);
                if (result != null)
                    return result;
            }

            UserProfileResponse response = await context.Client.SendAsync<UserProfileResponse>(
                new UserProfileMessage(new uint[] { id }, true, true), cancellationToken).ConfigureAwait(false);
            return response?.UserProfiles?.FirstOrDefault(u => u.ID == id);
        }


        // responding
        /// <summary>Sends a text message response message to group or user.</summary>
        /// <param name="incomingMessage">Message the user or group sent to the client.</param>
        /// <param name="text">Content of the message.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> ReplyTextAsync(this ICommandContext context, string text, CancellationToken cancellationToken = default)
        => context.Client.SendAsync<ChatResponse>(new ChatMessage(context.Message.IsGroupMessage ? context.Message.RecipientID : context.Message.SenderID.Value, context.Message.IsGroupMessage, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text)), cancellationToken);
        /// <summary>Sends an image response message to group or user.</summary>
        /// <param name="incomingMessage">Message the user or group sent to the client.</param>
        /// <param name="imageBytes">Bytes of the image to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> ReplyImageAsync(this ICommandContext context, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default)
            => context.Client.SendAsync<ChatResponse>(new ChatMessage(context.Message.IsGroupMessage ? context.Message.RecipientID : context.Message.SenderID.Value, context.Message.IsGroupMessage, ChatMessageTypes.Image, imageBytes), cancellationToken);
        /// <summary>Sends a voice response message to group or user.</summary>
        /// <param name="incomingMessage">Message the user or group sent to the client.</param>
        /// <param name="voiceBytes">Bytes of the voice to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> ReplyVoiceAsync(this ICommandContext context, IEnumerable<byte> voiceBytes, CancellationToken cancellationToken = default)
            => context.Client.SendAsync<ChatResponse>(new ChatMessage(context.Message.IsGroupMessage ? context.Message.RecipientID : context.Message.SenderID.Value, context.Message.IsGroupMessage, ChatMessageTypes.Voice, voiceBytes), cancellationToken);
    }
}
