using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    public static class Sender
    {
        /** LOGGING IN AND OUT **/
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


        /** NOTIFICATIONS **/
        #region Notifications
        public static async Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, WolfLanguage language, WolfDevice device, CancellationToken cancellationToken = default)
        {
            ListNotificationsResponse response = await client.SendAsync<ListNotificationsResponse>(new ListNotificationsMessage(language, device), cancellationToken).ConfigureAwait(false);
            return response.Notifications?.Any() == true ? response.Notifications : Enumerable.Empty<WolfNotification>();
        }
        public static Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.GetNotificationsAsync(WolfLanguage.English, WolfDevice.Bot, cancellationToken);

        public static Task ClearNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.SendAsync(new ClearNotificationsMessage(), cancellationToken);
        #endregion


        /** USERS **/
        #region Users
        public static async Task<IEnumerable<WolfUser>> GetUsersAsync(this IWolfClient client, IEnumerable<uint> userIDs, CancellationToken cancellationToken = default)
        {
            if (userIDs?.Any() != true)
                throw new ArgumentException("There must be at least one user ID to retrieve", nameof(userIDs));

            // get as many users from cache as possible
            List<WolfUser> results = new List<WolfUser>(userIDs.Count());
            if (client is IWolfClientCacheAccessor cache)
                results.AddRange(userIDs.Select(uID => cache.GetCachedUser(uID)).Where(u => u != null));

            // get the ones that aren't in cache from the server
            IEnumerable<uint> toRequest = userIDs.Except(results.Select(u => u.ID));
            if (toRequest.Any())
            {
                UserProfileResponse response = await client.SendAsync<UserProfileResponse>(
                    new UserProfileMessage(toRequest, true, true), cancellationToken).ConfigureAwait(false);
                results.AddRange(response.UserProfiles);
            }
            return results;
        }

        public static Task<WolfUser> GetCurrentUserAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            if (client.CurrentUserID == null)
                throw new InvalidOperationException("Not logged in");
            return client.GetUserAsync(client.CurrentUserID.Value, cancellationToken);
        }

        public static async Task<WolfUser> GetUserAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfUser> users = await client.GetUsersAsync(new uint[] { userID }, cancellationToken).ConfigureAwait(false);
            return users.FirstOrDefault();
        }

        public static async Task<IEnumerable<WolfUser>> GetContactListAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            ListContactsResponse response = await client.SendAsync<ListContactsResponse>(new ListContactsMessage(), cancellationToken).ConfigureAwait(false);
            return await client.GetUsersAsync(response.ContactIDs, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        
        /** HISTORY **/
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

        
        /** GROUPS **/
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

        // get groups
        public static async Task<IEnumerable<WolfGroup>> GetGroupsAsync(this IWolfClient client, IEnumerable<uint> groupIDs, CancellationToken cancellationToken = default)
        {
            if (groupIDs?.Any() != true)
                throw new ArgumentException("There must be at least one group ID to retrieve", nameof(groupIDs));

            // get as many groups from cache as possible
            List<WolfGroup> results = new List<WolfGroup>(groupIDs.Count());
            if (client is IWolfClientCacheAccessor cache)
                results.AddRange(groupIDs.Select(gID => cache.GetCachedGroup(gID)).Where(g => g != null));

            // get the ones that aren't in cache from the server
            IEnumerable<uint> toRequest = groupIDs.Except(results.Select(u => u.ID));
            if (toRequest.Any())
            {
                GroupProfileResponse response = await client.SendAsync<GroupProfileResponse>(
                    new GroupProfileMessage(toRequest, true), cancellationToken).ConfigureAwait(false);
                results.AddRange(response.GroupProfiles);

                foreach (WolfGroup group in response.GroupProfiles)
                {
                    // request members list for groups not present in cache
                    try
                    {
                        ListGroupMembersResponse membersResponse = await client.SendAsync<ListGroupMembersResponse>(
                            new ListGroupMembersMessage(group.ID), cancellationToken).ConfigureAwait(false);
                        // client should be configured to intercept this response
                        // however, just in case it's not (like when caching is disabled), do it here as well
                        if (membersResponse?.GroupMembers != null)
                            EntityModificationHelper.ReplaceAllGroupMembers(group, membersResponse.GroupMembers);
                    }
                    // handle case when requesting profiles for group the user is not in
                    catch (MessageSendingException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden) { }
                    catch (NotSupportedException) { }
                }
            }
            return results;
        }

        public static async Task<WolfGroup> GetGroupAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfGroup> groups = await client.GetGroupsAsync(new uint[] { groupID }, cancellationToken).ConfigureAwait(false);
            return groups.FirstOrDefault();
        }

        public static async Task<IEnumerable<WolfGroup>> GetCurrentUserGroupsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            ListUserGroupsResponse response = await client.SendAsync<ListUserGroupsResponse>(
                new ListUserGroupsMessage(), cancellationToken).ConfigureAwait(false);
            return await client.GetGroupsAsync(response.UserGroupIDs, cancellationToken).ConfigureAwait(false);
        }
        #endregion


        /** CHARMS **/
        #region Charms
        // charms
        public static async Task<IEnumerable<WolfCharm>> GetCharmsAsync(this IWolfClient client, IEnumerable<uint> charmIDs, CancellationToken cancellationToken = default)
        {
            if (charmIDs != null && !charmIDs.Any())
                charmIDs = null;

            // get as many charms from cache as possible
            List<WolfCharm> results = new List<WolfCharm>(charmIDs?.Count() ?? 600);
            if (charmIDs != null && client is IWolfClientCacheAccessor cache)
                results.AddRange(charmIDs.Select(cID => cache.GetCachedCharm(cID)).Where(c => c != null));

            // get the ones that aren't in cache from the server
            IEnumerable<uint> toRequest = charmIDs?.Except(results.Select(u => u.ID));
            if (toRequest == null || toRequest.Any())
            {
                ListCharmsResponse response = await client.SendAsync<ListCharmsResponse>(
                    new ListCharmsMessage(toRequest), cancellationToken).ConfigureAwait(false);
                results.AddRange(response.Charms);
            }
            return results;
        }
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


        /** ADMIN ACTIONS **/
        #region Admin Actions
        public static Task AdminUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Admin), cancellationToken);
        public static Task ModUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Mod), cancellationToken);
        public static Task ResetUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.User), cancellationToken);
        public static Task SilenceUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Silenced), cancellationToken);
        public static Task KickUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.NotMember), cancellationToken);
        public static Task BanUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Banned), cancellationToken);
        #endregion


        /** PROFILE UPDATES **/
        #region Profile Updates
        public static async Task<WolfUser> UpdateProfileAsync(this IWolfClient client, Action<UserUpdateMessage.Builder> update, CancellationToken cancellationToken = default)
        {
            WolfUser currentUser = await client.GetCurrentUserAsync(cancellationToken).ConfigureAwait(false);
            UserUpdateMessage.Builder updateBuilder = new UserUpdateMessage.Builder(currentUser);
            update?.Invoke(updateBuilder);
            UserUpdateResponse response = await client.SendAsync<UserUpdateResponse>(updateBuilder.Build(), cancellationToken).ConfigureAwait(false);
            return response.UserProfile;
        }

        public static Task<WolfUser> UpdateNicknameAsync(this IWolfClient client, string newNickname, CancellationToken cancellationToken = default)
            => client.UpdateProfileAsync(user => user.Nickname = newNickname, cancellationToken);

        public static Task<WolfUser> UpdateStatusAsync(this IWolfClient client, string newStatus, CancellationToken cancellationToken = default)
            => client.UpdateProfileAsync(user => user.Status = newStatus, cancellationToken);
        #endregion
    }
}
