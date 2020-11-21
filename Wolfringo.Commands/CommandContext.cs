using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands
{
    /// <inheritdoc/>
    public class CommandContext : ICommandContext
    {
        /// <inheritdoc/>
        IChatMessage ICommandContext.Message => this.Message;
        /// <summary>Chat message that triggered the command.</summary>
        public ChatMessage Message { get; }
        /// <inheritdoc/>
        public IWolfClient Client { get; }
        /// <inheritdoc/>
        public ICommandsOptions Options { get; }

        /// <summary>Whether the message was sent in a group.</summary>
        public bool IsGroup => this.Message.IsGroupMessage;
        /// <summary>Whether the message was sent in PM.</summary>
        public bool IsPrivate => this.Message.IsPrivateMessage;

        /// <summary>Creates a command context.</summary>
        /// <param name="message">Chat message that triggered the command.</param>
        /// <param name="client">WOLF client that received the message.</param>
        /// <param name="options">Default options to use for processing the command.</param>
        public CommandContext(ChatMessage message, IWolfClient client, ICommandsOptions options)
        {
            this.Message = message;
            this.Client = client;
            this.Options = options;
        }

        /// <summary>Gets profile of the user that sent the command.</summary>
        /// <param name="cancellationToken">Token to cancel server request with.</param>
        /// <returns>Profile of the user that sent the command</returns>
        /// <seealso cref="GetRecipientAsync{T}(CancellationToken)"/>
        public Task<WolfUser> GetSenderAsync(CancellationToken cancellationToken = default)
            => GetUserAsync(this.Message.SenderID.Value, cancellationToken);

        /// <summary>Gets profile of the recipient of the message.</summary>
        /// <typeparam name="T">Type of recipient, for example <see cref="WolfUser"/> or <see cref="WolfGroup"/>.</typeparam>
        /// <param name="cancellationToken">Token to cancel server request with.</param>
        /// <returns>Profile of the message's recipient; if <typeparamref name="T"/> does not match the message type, null will be returned.</returns>
        /// <seealso cref="GetSenderAsync(CancellationToken)"/>
        public async Task<T> GetRecipientAsync<T>(CancellationToken cancellationToken = default) where T : class, IWolfEntity
        {
            if (this.IsGroup)
            {
                if (this.Client is IWolfClientCacheAccessor cache)
                {
                    WolfGroup result = cache.GetCachedGroup(this.Message.RecipientID);
                    if (result != null)
                        return result as T;
                }

                GroupProfileResponse response = await this.Client.SendAsync<GroupProfileResponse>(
                    new GroupProfileMessage(new uint[] { this.Message.RecipientID }, true), cancellationToken).ConfigureAwait(false);
                return response?.GroupProfiles?.FirstOrDefault(g => g.ID == this.Message.RecipientID) as T;
            }
            else
                return await GetUserAsync(this.Message.RecipientID, cancellationToken) as T;
        }

        private async Task<WolfUser> GetUserAsync(uint id, CancellationToken cancellationToken = default)
        {
            if (this.Client is IWolfClientCacheAccessor cache)
            {
                WolfUser result = cache.GetCachedUser(id);
                if (result != null)
                    return result;
            }

            UserProfileResponse response = await this.Client.SendAsync<UserProfileResponse>(
                new UserProfileMessage(new uint[] { id }, true, true), cancellationToken).ConfigureAwait(false);
            return response?.UserProfiles?.FirstOrDefault(u => u.ID == id);
        }
    }
}
