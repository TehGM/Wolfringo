using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands
{
    public class CommandContext : ICommandContext
    {
        IChatMessage ICommandContext.Message => this.Message;
        public ChatMessage Message { get; }
        public IWolfClient Client { get; }
        public ICommandsOptions Options { get; }

        public bool IsGroup => this.Message.IsGroupMessage;
        public bool IsPrivate => this.Message.IsPrivateMessage;

        public CommandContext(ChatMessage message, IWolfClient client, ICommandsOptions options)
        {
            this.Message = message;
            this.Client = client;
            this.Options = options;
        }

        public Task<WolfUser> GetSenderAsync(CancellationToken cancellationToken = default)
            => GetUserAsync(this.Message.SenderID.Value, cancellationToken);

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
