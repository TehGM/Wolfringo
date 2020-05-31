using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    public static class Sender
    {
        #region Logging in and out
        public static async Task<LoginResponse> LoginAsync(this IWolfClient client, string login, string password, bool isPasswordAlreadyHashed = false, CancellationToken cancellationToken = default)
        {
            LoginResponse response = await client.SendAsync<LoginResponse>(new LoginMessage(login, password, isPasswordAlreadyHashed), cancellationToken).ConfigureAwait(false);
            await client.SendAsync(new SubscribeToPmMessage(), cancellationToken).ConfigureAwait(false);
            await client.SendAsync(new SubscribeToGroupMessage(), cancellationToken).ConfigureAwait(false);
            return response;
        }

        public static Task SetOnlineStateAsync(this IWolfClient client, WolfOnlineState state, CancellationToken cancellationToken = default)
            => client.SendAsync(new OnlineStateUpdateMessage(state), cancellationToken);

        public static Task LogoutAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.SendAsync(new LogoutMessage(), cancellationToken);
        #endregion

        #region Notifications
        public static async Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, WolfLanguage language, WolfDevice device, CancellationToken cancellationToken = default)
        {
            ListNotificationsResponse response = await client.SendAsync<ListNotificationsResponse>(new ListNotificationsMessage(language, device), cancellationToken).ConfigureAwait(false);
            return response.Notifications?.Any() == true ? response.Notifications : Enumerable.Empty<WolfNotification>();
        }
        public static Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.GetNotificationsAsync(WolfLanguage.English, WolfDevice.Bot, cancellationToken);
        #endregion

        #region Contacts
        public static async Task<IEnumerable<WolfUser>> GetContactListAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            ListContactsResponse response = await client.SendAsync<ListContactsResponse>(new ListContactsMessage(), cancellationToken).ConfigureAwait(false);
            return await client.GetUsersAsync(response.ContactIDs, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region History
        // private history
        public static async Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, DateTime? beforeTime, bool oldestFirst, CancellationToken cancellationToken = default)
        {
            ChatHistoryResponse response = await client.SendAsync<ChatHistoryResponse>(
                new PrivateChatHistoryMessage(userID, beforeTime), cancellationToken).ConfigureAwait(false);
            return oldestFirst ?
                response.Messages.OrderBy(msg => msg.Timestamp) :
                response.Messages.OrderByDescending(msg => msg.Timestamp);
        }
        public static Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, DateTime? beforeTime, CancellationToken cancellationToken = default)
            => client.GetPrivateMessageHistoryAsync(userID, beforeTime, false, cancellationToken);
        public static Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, bool oldestFirst, CancellationToken cancellationToken = default)
            => client.GetPrivateMessageHistoryAsync(userID, null, oldestFirst, cancellationToken);
        public static Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.GetPrivateMessageHistoryAsync(userID, null, false, cancellationToken);

        // group history
        public static async Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, DateTime? beforeTime, bool oldestFirst, CancellationToken cancellationToken = default)
        {
            ChatHistoryResponse response = await client.SendAsync<ChatHistoryResponse>(
                new GroupChatHistoryMessage(groupID, beforeTime, oldestFirst), cancellationToken).ConfigureAwait(false);
            return oldestFirst ?
                response.Messages.OrderBy(msg => msg.Timestamp) :
                response.Messages.OrderByDescending(msg => msg.Timestamp);
        }
        public static Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, DateTime? beforeTime, CancellationToken cancellationToken = default)
            => client.GetGroupMessageHistoryAsync(groupID, beforeTime, false, cancellationToken);
        public static Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, bool oldestFirst, CancellationToken cancellationToken = default)
            => client.GetGroupMessageHistoryAsync(groupID, null, oldestFirst, cancellationToken);
        public static Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
            => client.GetGroupMessageHistoryAsync(groupID, null, false, cancellationToken);

        // recent conversations
        public static async Task<IEnumerable<IChatMessage>> GetRecentConversationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            RecentConversationsResponse response = await client.SendAsync<RecentConversationsResponse>(
                new RecentConversationsMessage(), cancellationToken).ConfigureAwait(false);
            return response.Messages;
        }
        #endregion

        #region Groups
        // join
        public static async Task<WolfGroup> JoinGroupAsync(this IWolfClient client, uint groupID, string password, CancellationToken cancellationToken = default)
        {
            await client.SendAsync(new GroupJoinMessage(groupID, password), cancellationToken).ConfigureAwait(false);
            return await client.GetGroupAsync(groupID, cancellationToken).ConfigureAwait(false);
        }
        public static Task<WolfGroup> JoinGroupAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
            => client.JoinGroupAsync(groupID, string.Empty, cancellationToken);

        // leave
        public static Task LeaveGroupAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupLeaveMessage(groupID), cancellationToken);
        public static Task LeaveGroupAsync(this IWolfClient client, WolfGroup group, CancellationToken cancellationToken = default)
            => client.LeaveGroupAsync(group.ID, cancellationToken);

        // get groups list
        public static async Task<IEnumerable<WolfGroup>> GetCurrentUserGroupsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            ListUserGroupsResponse response = await client.SendAsync<ListUserGroupsResponse>(
                new ListUserGroupsMessage(), cancellationToken).ConfigureAwait(false);
            return await client.GetGroupsAsync(response.UserGroupIDs, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Charms and Achievements
        // charms
        public static Task<IEnumerable<WolfCharm>> GetAllCharmsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.GetCharmsAsync(null, cancellationToken);
        public static async Task<WolfCharm> GetCharmAsync(this IWolfClient client, uint charmID, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfCharm> result = await client.GetCharmsAsync(new uint[] { charmID }, cancellationToken).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public static Task<CharmStatisticsResponse> GetUserCharmStatsAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync<CharmStatisticsResponse>(new CharmStatisticsMessage(userID), cancellationToken);
        #endregion
    }
}
