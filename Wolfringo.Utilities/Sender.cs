using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Utility helper for sending Wolf Client messages.</summary>
    /// <remarks><para>Sender utility is designed to abstract most common actions from the message and response objects.</para>
    /// <para>Methods are designed to make use of cached entities where possible.</para>
    /// <para>This utility does not support any custom message or response classes designed by the developer.</para></remarks>
    public static class Sender
    {
        /* LOGGING IN AND OUT */
        #region Logging in and out
        /// <summary>Log in.</summary>
        /// <param name="login">User email to use to login with.</param>
        /// <param name="password">User password.</param>
        /// <param name="isPasswordAlreadyHashed">Whether <paramref name="password"/> is provided already hashed.</param>
        /// <returns>Response with login result.</returns>
        /// <seealso cref="LogoutAsync(IWolfClient, CancellationToken)"/>
        public static async Task<LoginResponse> LoginAsync(this IWolfClient client, string login, string password, bool isPasswordAlreadyHashed = false, CancellationToken cancellationToken = default)
        {
            LoginResponse response = await client.SendAsync<LoginResponse>(new LoginMessage(login, password, isPasswordAlreadyHashed), cancellationToken).ConfigureAwait(false);
            await client.SendAsync(new SubscribeToPmMessage(), cancellationToken).ConfigureAwait(false);
            await client.SendAsync(new SubscribeToGroupMessage(), cancellationToken).ConfigureAwait(false);
            return response;
        }

        /// <summary>Update current user's online state.</summary>
        /// <param name="state">Online state to set.</param>
        public static Task SetOnlineStateAsync(this IWolfClient client, WolfOnlineState state, CancellationToken cancellationToken = default)
            => client.SendAsync(new OnlineStateUpdateMessage(state), cancellationToken);

        /// <summary>Log out.</summary>
        /// <seealso cref="LoginAsync(IWolfClient, string, string, bool, CancellationToken)"/>
        public static Task LogoutAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.SendAsync(new LogoutMessage(), cancellationToken);
        #endregion


        /* NOTIFICATIONS */
        #region Notifications
        /// <summary>Get current user's notifications.</summary>
        /// <param name="language">Language to get notifications in.</param>
        /// <param name="device">Device to use.</param>
        /// <returns>Enumerable of retrieved notifications.</returns>
        /// <seealso cref="GetNotificationsAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="ClearNotificationsAsync(IWolfClient, CancellationToken)"/>
        public static async Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, WolfLanguage language, WolfDevice device, CancellationToken cancellationToken = default)
        {
            NotificationsListResponse response = await client.SendAsync<NotificationsListResponse>(new NotificationsListMessage(language, device), cancellationToken).ConfigureAwait(false);
            return response.Notifications?.Any() == true ? response.Notifications : Enumerable.Empty<WolfNotification>();
        }
        /// <summary>Get current user's notifications in English.</summary>
        /// <returns>Enumerable of retrieved notifications.</returns>
        /// <seealso cref="GetNotificationsAsync(IWolfClient, WolfLanguage, WolfDevice, CancellationToken)"/>
        /// <seealso cref="ClearNotificationsAsync(IWolfClient, CancellationToken)"/>
        public static Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.GetNotificationsAsync(WolfLanguage.English, WolfDevice.Bot, cancellationToken);

        /// <summary>Clear notifications list.</summary>
        /// <seealso cref="GetNotificationsAsync(IWolfClient, WolfLanguage, WolfDevice, CancellationToken)"/>
        /// <seealso cref="GetNotificationsAsync(IWolfClient, CancellationToken)"/>
        public static Task ClearNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.SendAsync(new NotificationsClearMessage(), cancellationToken);
        #endregion


        /* USERS */
        #region Users
        // retrieving
        /// <summary>Retrieve profiles of users by their IDs.</summary>
        /// <remarks>Users already cached will be retrieved from cache. Others will be requested from the server.</remarks>
        /// <param name="userIDs">IDs of users to retrieve.</param>
        /// <returns>Enumerable of retrieved users.</returns>
        /// <seealso cref="GetCurrentUserAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="GetUserAsync(IWolfClient, uint, CancellationToken)"/>
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

        /// <summary>Get profile of currently logged in user.</summary>
        /// <remarks>If user is already cached, cached instance will be returned. Otherwise a request to the server will be made.</remarks>
        /// <returns>Currently logged in user.</returns>
        /// <exception cref="InvalidOperationException">Not logged in.</exception>
        /// <seealso cref="GetUserAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetUsersAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        public static Task<WolfUser> GetCurrentUserAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            if (client.CurrentUserID == null)
                throw new InvalidOperationException("Not logged in");
            return client.GetUserAsync(client.CurrentUserID.Value, cancellationToken);
        }

        /// <summary>Get profile of specified user.</summary>
        /// <remarks>If user is already cached, cached instance will be returned. Otherwise a request to the server will be made.</remarks>
        /// <param name="userID">ID of user to retrieve.</param>
        /// <returns>Retrieved user.</returns>
        /// <seealso cref="GetUsersAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetCurrentUserAsync(IWolfClient, CancellationToken)"/>
        public static async Task<WolfUser> GetUserAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfUser> users = await client.GetUsersAsync(new uint[] { userID }, cancellationToken).ConfigureAwait(false);
            return users.FirstOrDefault();
        }

        // contacts
        /// <summary>Get current user's contact list.</summary>
        /// <remarks>Users already cached will be retrieved from cache. Others will be requested from the server.</remarks>
        /// <returns>Enumerable of profiles of all contacts.</returns>
        /// <seealso cref="AddContactAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="DeleteContactAsync(IWolfClient, uint, CancellationToken)"/>
        public static async Task<IEnumerable<WolfUser>> GetContactListAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            ContactListResponse response = await client.SendAsync<ContactListResponse>(new ContactListMessage(true), cancellationToken).ConfigureAwait(false);
            return await client.GetUsersAsync(response.ContactIDs, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Add contact.</summary>
        /// <remarks>Before adding, it's recommended to check if user is already a contact. 
        /// Adding user that's already in contacts list will result in an exception being thrown.</remarks>
        /// <param name="userID">ID of user to add.</param>
        /// <seealso cref="GetContactListAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="DeleteContactAsync(IWolfClient, uint, CancellationToken)"/>
        public static Task AddContactAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new ContactAddMessage(userID), cancellationToken);

        /// <summary>Delete contact.</summary>
        /// <param name="userID">ID of user to add.</param>
        /// <seealso cref="GetContactListAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="AddContactAsync(IWolfClient, uint, CancellationToken)"/>
        public static Task DeleteContactAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new ContactDeleteMessage(userID), cancellationToken);

        // blocking
        /// <summary>Get users blocked by current user.</summary>
        /// <remarks>Users already cached will be retrieved from cache. Others will be requested from the server.</remarks>
        /// <returns>Enumerable of profiles of blocked users.</returns>
        /// <seealso cref="BlockUserAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="UnblockUserAsync(IWolfClient, uint, CancellationToken)"/>
        public static async Task<IEnumerable<WolfUser>> GetBlockedUsersAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            BlockListResponse response = await client.SendAsync<BlockListResponse>(
                new BlockListMessage(), cancellationToken).ConfigureAwait(false);
            if (response.BlockedUsersIDs?.Any() != true)
                return Enumerable.Empty<WolfUser>();
            return await client.GetUsersAsync(response.BlockedUsersIDs, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Block user.</summary>
        /// <param name="userID">ID of user to block.</param>
        /// <seealso cref="GetBlockedUsersAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="UnblockUserAsync(IWolfClient, uint, CancellationToken)"/>
        public static Task BlockUserAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new BlockAddMessage(userID), cancellationToken);

        /// <summary>Unblock user.</summary>
        /// <param name="userID">ID of user to block.</param>
        /// <seealso cref="GetBlockedUsersAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="BlockUserAsync(IWolfClient, uint, CancellationToken)"/>
        public static Task UnblockUserAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new BlockDeleteMessage(userID), cancellationToken);

        // updating
        /// <summary>Update current user's profile.</summary>
        /// <param name="updates">Profile changes to apply.</param>
        /// <returns>Updated current user.</returns>
        /// <seealso cref="UpdateNicknameAsync(IWolfClient, string, CancellationToken)"/>
        /// <seealso cref="UpdateStatusAsync(IWolfClient, string, CancellationToken)"/>
        public static async Task<WolfUser> UpdateProfileAsync(this IWolfClient client, Action<UserUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            WolfUser currentUser = await client.GetCurrentUserAsync(cancellationToken).ConfigureAwait(false);
            UserUpdateMessage.Builder updateBuilder = new UserUpdateMessage.Builder(currentUser);
            updates?.Invoke(updateBuilder);
            UserUpdateResponse response = await client.SendAsync<UserUpdateResponse>(updateBuilder.Build(), cancellationToken).ConfigureAwait(false);
            return response.UserProfile;
        }

        /// <summary>Update current user's nickname.</summary>
        /// <param name="newNickname">Nickname to set.</param>
        /// <returns>Updated current user.</returns>
        /// <seealso cref="UpdateProfileAsync(IWolfClient, Action{UserUpdateMessage.Builder}, CancellationToken)"/>
        /// <seealso cref="UpdateStatusAsync(IWolfClient, string, CancellationToken)"/>
        public static Task<WolfUser> UpdateNicknameAsync(this IWolfClient client, string newNickname, CancellationToken cancellationToken = default)
            => client.UpdateProfileAsync(user => user.Nickname = newNickname, cancellationToken);

        /// <summary>Update current user's status.</summary>
        /// <param name="newNickname">Status to set.</param>
        /// <returns>Updated current user.</returns>
        /// <seealso cref="UpdateProfileAsync(IWolfClient, Action{UserUpdateMessage.Builder}, CancellationToken)"/>
        /// <seealso cref="UpdateNicknameAsync(IWolfClient, string, CancellationToken)"/>
        public static Task<WolfUser> UpdateStatusAsync(this IWolfClient client, string newStatus, CancellationToken cancellationToken = default)
            => client.UpdateProfileAsync(user => user.Status = newStatus, cancellationToken);
        #endregion


        /* HISTORY */
        #region History
        // private history
        /// <summary>Retrieve private messages history.</summary>
        /// <param name="userID">ID of user to get message history with.</param>
        /// <param name="beforeTime">Timestamp of oldest already retrieved message; null to retrieve from newest.</param>
        /// <param name="oldestFirst">Whether to order retrieved messages from oldest to newest.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetGroupMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static async Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, DateTime? beforeTime, bool oldestFirst, CancellationToken cancellationToken = default)
        {
            ChatHistoryResponse response = await client.SendAsync<ChatHistoryResponse>(
                new PrivateChatHistoryMessage(userID, beforeTime), cancellationToken).ConfigureAwait(false);
            return oldestFirst ?
                response.Messages.OrderBy(msg => msg.Timestamp) :
                response.Messages.OrderByDescending(msg => msg.Timestamp);
        }
        /// <summary>Retrieve private messages history, ordered from newest to oldest.</summary>
        /// <param name="userID">ID of user to get message history with.</param>
        /// <param name="beforeTime">Timestamp of oldest already retrieved message; null to retrieve from newest.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetGroupMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, DateTime? beforeTime, CancellationToken cancellationToken = default)
            => client.GetPrivateMessageHistoryAsync(userID, beforeTime, false, cancellationToken);
        /// <summary>Retrieve private messages history, starting with most recent message.</summary>
        /// <param name="userID">ID of user to get message history with.</param>
        /// <param name="oldestFirst">Whether to order retrieved messages from oldest to newest.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetGroupMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, bool oldestFirst, CancellationToken cancellationToken = default)
            => client.GetPrivateMessageHistoryAsync(userID, null, oldestFirst, cancellationToken);
        /// <summary>Retrieve private messages history, ordered from newest to oldest, starting with most recent message.</summary>
        /// <param name="userID">ID of user to get message history with.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetGroupMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static Task<IEnumerable<IChatMessage>> GetPrivateMessageHistoryAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.GetPrivateMessageHistoryAsync(userID, null, false, cancellationToken);

        // group history
        /// <summary>Retrieve group messages history.</summary>
        /// <param name="groupID">ID of group to get message history from.</param>
        /// <param name="beforeTime">Timestamp of oldest already retrieved message; null to retrieve from newest.</param>
        /// <param name="oldestFirst">Whether to order retrieved messages from oldest to newest.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetPrivateMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static async Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, DateTime? beforeTime, bool oldestFirst, CancellationToken cancellationToken = default)
        {
            ChatHistoryResponse response = await client.SendAsync<ChatHistoryResponse>(
                new GroupChatHistoryMessage(groupID, beforeTime, oldestFirst), cancellationToken).ConfigureAwait(false);
            return oldestFirst ?
                response.Messages.OrderBy(msg => msg.Timestamp) :
                response.Messages.OrderByDescending(msg => msg.Timestamp);
        }
        /// <summary>Retrieve group messages history, ordered from newest to oldest.</summary>
        /// <param name="groupID">ID of group to get message history from.</param>
        /// <param name="beforeTime">Timestamp of oldest already retrieved message; null to retrieve from newest.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetPrivateMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, DateTime? beforeTime, CancellationToken cancellationToken = default)
            => client.GetGroupMessageHistoryAsync(groupID, beforeTime, false, cancellationToken);
        /// <summary>Retrieve group messages history, starting with most recent message.</summary>
        /// <param name="groupID">ID of group to get message history from.</param>
        /// <param name="oldestFirst">Whether to order retrieved messages from oldest to newest.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetPrivateMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, bool oldestFirst, CancellationToken cancellationToken = default)
            => client.GetGroupMessageHistoryAsync(groupID, null, oldestFirst, cancellationToken);
        /// <summary>Retrieve group messages history, ordered from newest to oldest, starting with most recent message.</summary>
        /// <param name="groupID">ID of group to get message history from.</param>
        /// <returns>Enumerable of retrieved messages.</returns>
        /// <seealso cref="GetPrivateMessageHistoryAsync(IWolfClient, uint, DateTime?, bool, CancellationToken)"/>
        public static Task<IEnumerable<IChatMessage>> GetGroupMessageHistoryAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
            => client.GetGroupMessageHistoryAsync(groupID, null, false, cancellationToken);

        // recent conversations
        /// <summary>Retrieve list of most recent messages.</summary>
        /// <returns>Enumerable with most recent messages.</returns>
        public static async Task<IEnumerable<IChatMessage>> GetRecentConversationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            RecentConversationsResponse response = await client.SendAsync<RecentConversationsResponse>(
                new RecentConversationsMessage(), cancellationToken).ConfigureAwait(false);
            return response.Messages;
        }
        #endregion


        /* GROUPS */
        #region Groups
        // join
        /// <summary>Join a group.</summary>
        /// <param name="groupID">ID of group to join.</param>
        /// <param name="password">Password to use when joining.</param>
        /// <returns>Joined group's profile.</returns>
        /// <seealso cref="LeaveGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        public static async Task<WolfGroup> JoinGroupAsync(this IWolfClient client, uint groupID, string password, CancellationToken cancellationToken = default)
        {
            await client.SendAsync(new GroupJoinMessage(groupID, password), cancellationToken).ConfigureAwait(false);
            WolfGroup group = await client.GetGroupAsync(groupID, cancellationToken).ConfigureAwait(false);
            if (group.Members?.Any() != true)
                await client.RequestGroupMembersAsync(group, cancellationToken).ConfigureAwait(false);
            return group;
        }
        /// <summary>Join a group.</summary>
        /// <param name="groupID">ID of group to join.</param>
        /// <returns>Joined group's profile.</returns>
        /// <seealso cref="LeaveGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        public static Task<WolfGroup> JoinGroupAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
            => client.JoinGroupAsync(groupID, string.Empty, cancellationToken);
        /// <summary>Join group.</summary>
        /// <param name="groupName">ID of the group to join.</param>
        /// <param name="password">Password to use when joining.</param>
        /// <returns>Joined group's profile.</returns>
        /// <seealso cref="LeaveGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        public static async Task<WolfGroup> JoinGroupAsync(this IWolfClient client, string groupName, string password, CancellationToken cancellationToken = default)
        {
            await client.SendAsync(new GroupJoinMessage(groupName, password), cancellationToken).ConfigureAwait(false);
            WolfGroup group = await client.GetGroupAsync(groupName, cancellationToken).ConfigureAwait(false);
            if (group.Members?.Any() != true)
                await client.RequestGroupMembersAsync(group, cancellationToken).ConfigureAwait(false);
            return group;
        }
        /// <summary>Join a group.</summary>
        /// <param name="groupName">Name of the group to join.</param>
        /// <returns>Joined group's profile.</returns>
        /// <seealso cref="LeaveGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        public static Task<WolfGroup> JoinGroupAsync(this IWolfClient client, string groupName, CancellationToken cancellationToken = default)
            => client.JoinGroupAsync(groupName, string.Empty, cancellationToken);

        // leave
        /// <summary>Leave a group.</summary>
        /// <param name="groupID">ID of group to leave.</param>
        /// <seealso cref="JoinGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        public static Task LeaveGroupAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupLeaveMessage(groupID), cancellationToken);

        // get groups
        /// <summary>Retrieve profiles of groups by their IDs.</summary>
        /// <remarks><para>Groups already cached will be retrieved from cache. Others will be requested from the server.</para>
        /// <para>Groups will be retrieved with members list populated.</para></remarks>
        /// <param name="groupIDs">IDs of groups to retrieve.</param>
        /// <returns>Enumerable of retrieved groups.</returns>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupStatisticsAsync(IWolfClient, uint, CancellationToken)"/>
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
                    await client.RequestGroupMembersAsync(group, cancellationToken).ConfigureAwait(false);
                }
            }
            return results;
        }

        private static async Task<IEnumerable<WolfGroupMember>> RequestGroupMembersAsync(this IWolfClient client, WolfGroup group, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                GroupMembersListResponse membersResponse = await client.SendAsync<GroupMembersListResponse>(
                    new GroupMembersListMessage(group.ID), cancellationToken).ConfigureAwait(false);
                // client should be configured to intercept this response
                // however, just in case it's not (like when caching is disabled), do it here as well
                if (membersResponse?.GroupMembers != null)
                {
                    EntityModificationHelper.ReplaceAllGroupMembers(group, membersResponse.GroupMembers);
                    return membersResponse.GroupMembers;
                }
            }
            // handle case when requesting profiles for group the user is not in
            catch (MessageSendingException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden) { }
            catch (NotSupportedException) { }
            return null;
        }

        /// <summary>Get profile of specified group.</summary>
        /// <remarks><para>If group is already cached, cached instance will be returned. Otherwise a request to the server will be made.</para>
        /// <para>Group will be retrieved with members list populated.</para></remarks>
        /// <param name="groupID">ID of group to retrieve.</param>
        /// <returns>Retrieved group.</returns>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetGroupStatisticsAsync(IWolfClient, uint, CancellationToken)"/>
        public static async Task<WolfGroup> GetGroupAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfGroup> groups = await client.GetGroupsAsync(new uint[] { groupID }, cancellationToken).ConfigureAwait(false);
            return groups.FirstOrDefault();
        }

        /// <summary>Get profile of specified group.</summary>
        /// <remarks><para>If group is already cached, cached instance will be returned. Otherwise a request to the server will be made.</para>
        /// <para>Group will be retrieved with members list populated.</para></remarks>
        /// <param name="groupID">ID of group to retrieve.</param>
        /// <returns>Retrieved group.</returns>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetGroupStatisticsAsync(IWolfClient, uint, CancellationToken)"/>
        public static async Task<WolfGroup> GetGroupAsync(this IWolfClient client, string groupName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentNullException(nameof(groupName));

            string trimmedName = groupName.Trim();

            // check cache first
            WolfGroup result = null;
            if (client is IWolfClientCacheAccessor cache)
                result = cache.GetCachedGroup(trimmedName);
            if (result != null)
                return result;

            // if cache misses, need to request from the server
            GroupProfileResponse response = await client.SendAsync<GroupProfileResponse>(
                new GroupProfileMessage(trimmedName), cancellationToken).ConfigureAwait(false);
            return response?.GroupProfiles?.FirstOrDefault();
        }

        /// <summary>Retrieve profiles of groups current user is in.</summary>
        /// <remarks><para>Groups already cached will be retrieved from cache. Others will be requested from the server.</para>
        /// <para>Groups will be retrieved with members list populated.</para></remarks>
        /// <returns>Enumerable of retrieved groups.</returns>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        public static async Task<IEnumerable<WolfGroup>> GetCurrentUserGroupsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            GroupListResponse response = await client.SendAsync<GroupListResponse>(
                new GroupListMessage(), cancellationToken).ConfigureAwait(false);
            return await client.GetGroupsAsync(response.UserGroupIDs, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Gets statistics of a group.</summary>
        /// <param name="groupID">ID of group to retrieve statistics for.</param>
        /// <returns>Retrieved group statistics.</returns>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetCurrentUserGroupsAsync(IWolfClient, CancellationToken)"/>
        public static async Task<WolfGroupStatistics> GetGroupStatisticsAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
        {
            GroupStatisticsResponse response = await client.SendAsync<GroupStatisticsResponse>(
                new GroupStatisticsMessage(groupID), cancellationToken).ConfigureAwait(false);
            return response.GroupStatistics;
        }

        // create
        /// <summary>Creates a new group.</summary>
        /// <param name="groupName">Name of group to create.</param>
        /// <param name="groupDescription">Short description for the group.</param>
        /// <param name="groupSettings">Group profile settings to apply.</param>
        /// <returns>Profile of created group.</returns>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="UpdateGroupAsync(IWolfClient, uint, Action{GroupUpdateMessage.Builder}, CancellationToken)"/>
        public static async Task<WolfGroup> CreateGroupAsync(this IWolfClient client, string groupName, string groupDescription, Action<GroupCreateMessage.Builder> groupSettings, CancellationToken cancellationToken = default)
        {
            GroupCreateMessage.Builder builder = new GroupCreateMessage.Builder(groupName, groupDescription);
            groupSettings?.Invoke(builder);
            GroupEditResponse response = await client.SendAsync<GroupEditResponse>(builder.Build(), cancellationToken).ConfigureAwait(false);
            return response.GroupProfile;
        }

        /// <summary>Creates a new group.</summary>
        /// <param name="groupName">Name of group to create.</param>
        /// <param name="groupDescription">Short description for the group.</param>
        /// <returns>Profile of created group.</returns>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="UpdateGroupAsync(IWolfClient, uint, Action{GroupUpdateMessage.Builder}, CancellationToken)"/>
        public static Task<WolfGroup> CreateGroupAsync(this IWolfClient client, string groupName, string groupDescription, CancellationToken cancellationToken = default)
            => client.CreateGroupAsync(groupName, groupDescription, null, cancellationToken);

        // update
        /// <summary>Updates group profile.</summary>
        /// <param name="groupID">ID of group to update.</param>
        /// <param name="updates">Group profile changes to apply.</param>
        /// <returns>Profile of updated group.</returns>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="CreateGroupAsync(IWolfClient, string, string, Action{GroupCreateMessage.Builder}, CancellationToken)"/>
        /// <seealso cref="UpdateGroupAudioConfigAsync(IWolfClient, uint, Action{GroupAudioUpdateMessage.Builder}, CancellationToken)"/>
        public static async Task<WolfGroup> UpdateGroupAsync(this IWolfClient client, uint groupID,
            Action<GroupUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            WolfGroup group = await client.GetGroupAsync(groupID, cancellationToken).ConfigureAwait(false);
            GroupUpdateMessage.Builder builder = new GroupUpdateMessage.Builder(group);
            updates?.Invoke(builder);
            GroupEditResponse response = await client.SendAsync<GroupEditResponse>(builder.Build(), cancellationToken).ConfigureAwait(false);
            return response.GroupProfile;
        }

        /// <summary>Updates group audio configuration.</summary>
        /// <param name="groupID">ID of group to update.</param>
        /// <param name="updates">Group audio configuration changes to apply.</param>
        /// <returns>Updated audio configuration.</returns>
        /// <seealso cref="GetGroupAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetGroupsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="CreateGroupAsync(IWolfClient, string, string, Action{GroupCreateMessage.Builder}, CancellationToken)"/>
        /// <seealso cref="UpdateGroupAsync(IWolfClient, uint, Action{GroupUpdateMessage.Builder}, CancellationToken)"/>
        public static async Task<WolfGroup.WolfGroupAudioConfig> UpdateGroupAudioConfigAsync(this IWolfClient client, uint groupID,
            Action<GroupAudioUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            WolfGroup group = await client.GetGroupAsync(groupID, cancellationToken).ConfigureAwait(false);
            GroupAudioUpdateMessage.Builder builder = new GroupAudioUpdateMessage.Builder(group.AudioConfig);
            updates?.Invoke(builder);
            GroupAudioUpdateResponse response = await client.SendAsync<GroupAudioUpdateResponse>(
                builder.Build(), cancellationToken).ConfigureAwait(false);
            return response.AudioConfig;
        }
        #endregion


        /* CHARMS */
        #region Charms
        // charms
        /// <summary>Retrieve charms by their IDs.</summary>
        /// <remarks>Charms already cached will be retrieved from cache. Others will be requested from the server.</remarks>
        /// <param name="charmIDs">IDs of charms to retrieve.</param>
        /// <returns>Enumerable of retrieved charms.</returns>
        /// <seealso cref="GetAllCharmsAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="GetCharmAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetUserCurrentCharm(IWolfClient, uint, CancellationToken)"/>
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
                CharmListResponse response = await client.SendAsync<CharmListResponse>(
                    new CharmListMessage(toRequest), cancellationToken).ConfigureAwait(false);
                results.AddRange(response.Charms);
            }
            return results;
        }
        /// <summary>Retrieve all charms.</summary>
        /// <remarks>Charms already cached will be retrieved from cache. Others will be requested from the server.</remarks>
        /// <returns>Enumerable of retrieved charms.</returns>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetCharmAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetUserCurrentCharm(IWolfClient, uint, CancellationToken)"/>
        public static Task<IEnumerable<WolfCharm>> GetAllCharmsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.GetCharmsAsync(null, cancellationToken);
        /// <summary>Get charm by ID.</summary>
        /// <remarks>If charm is already cached, cached instance will be returned. Otherwise a request to the server will be made.</remarks>
        /// <param name="charmID">ID of charm to retrieve.</param>
        /// <returns>Retrieved charm.</returns>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetAllCharmsAsync(IWolfClient, CancellationToken)"/>
        /// <seealso cref="GetUserCurrentCharm(IWolfClient, uint, CancellationToken)"/>
        public static async Task<WolfCharm> GetCharmAsync(this IWolfClient client, uint charmID, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfCharm> result = await client.GetCharmsAsync(new uint[] { charmID }, cancellationToken).ConfigureAwait(false);
            return result.FirstOrDefault();
        }


        // charm ownership
        /// <summary>Get user's charm statistics.</summary>
        /// <param name="userID">ID of user to retrieve charm statistics of.</param>
        /// <returns>User's charm statistics.</returns>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetUserActiveCharmsAsync(IWolfClient, uint, CancellationToken)"/>
        public static Task<CharmStatisticsResponse> GetUserCharmStatsAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync<CharmStatisticsResponse>(new CharmStatisticsMessage(userID), cancellationToken);

        /// <summary>Get user's selected charm.</summary>
        /// <remarks><para>If charm is already cached, cached instance will be returned. Otherwise a request to the server will be made.</para>
        /// <para>If user is already cached, cached instance will be returned. Otherwise a request to the server will be made.</para></remarks>
        /// <param name="userID">ID of user to retrieve selected charm of.</param>
        /// <returns>User's currently selected charm.</returns>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetUserActiveCharmsAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="SetActiveCharmAsync(IWolfClient, uint, CancellationToken)"/>
        public static async Task<WolfCharm> GetUserCurrentCharm(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            WolfUser user = await client.GetUserAsync(userID, cancellationToken).ConfigureAwait(false);
            if (user == null)
                throw new KeyNotFoundException($"User {userID} not found");
            if (user.ActiveCharmID == null)
                return null;
            return await client.GetCharmAsync(user.ActiveCharmID.Value, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get user's currently owned charms.</summary>
        /// <param name="userID">ID of user to retrieve owned charms of.</param>
        /// <returns>Enumerable of owned charms subscriptions.</returns>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetUserCurrentCharm(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetUserExpiredCharmsAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="SetActiveCharmAsync(IWolfClient, uint, CancellationToken)"/>
        public static async Task<IEnumerable<WolfCharmSubscription>> GetUserActiveCharmsAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            UserCharmsListResponse response = await client.SendAsync<UserCharmsListResponse>(
                new UserActiveCharmsListMessage(userID), cancellationToken).ConfigureAwait(false);
            return response.Charms;
        }
        /// <summary>Get user's expired charms.</summary>
        /// <param name="userID">ID of user to retrieve expired charms of.</param>
        /// <returns>Enumerable of expired charms subscriptions.</returns>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetUserActiveCharmsAsync(IWolfClient, uint, CancellationToken)"/>
        public static async Task<IEnumerable<WolfCharmSubscription>> GetUserExpiredCharmsAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            UserCharmsListResponse response = await client.SendAsync<UserCharmsListResponse>(
                new UserExpiredCharmsListMessage(userID), cancellationToken).ConfigureAwait(false);
            return response.Charms;
        }

        // setting charm
        /// <summary>Set current user's selected charm.</summary>
        /// <param name="charmID">ID of charm to set as active.</param>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetUserActiveCharmsAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetUserCurrentCharm(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="RemoveActiveCharmAsync(IWolfClient, CancellationToken)"/>
        public static Task SetActiveCharmAsync(this IWolfClient client, uint charmID, CancellationToken cancellationToken = default)
            => client.SendAsync(new UserCharmsSelectMessage(new Dictionary<int, uint>() { { 0, charmID } }), cancellationToken);
        /// <summary>Remove current user's selected charm.</summary>
        /// <seealso cref="GetCharmsAsync(IWolfClient, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetUserActiveCharmsAsync(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="GetUserCurrentCharm(IWolfClient, uint, CancellationToken)"/>
        /// <seealso cref="SetActiveCharmAsync(IWolfClient, uint, CancellationToken)"/>
        public static Task RemoveActiveCharmAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.SendAsync(new UserCharmsSelectMessage(new Dictionary<int, uint>()), cancellationToken);
        #endregion


        /* ACHIEVEMENTS */
        #region Achievements
        /// <summary>Retrieve achievements by their IDs.</summary>
        /// <remarks><para>Achievements already cached will be retrieved from cache.</para>
        /// <para>Due to the construction of the protocol, if any achievement is not cached, the client will request all achievements again.</para>
        /// <para>All child achievements will be surfaced to the top level, so can be accessed by direct enumerable queries.</para></remarks>
        /// <param name="language">Language to retrieve achievements in.</param>
        /// <param name="achievementIDs">IDs of achievements to retrieve.</param>
        /// <returns>Enumerable of retrieved achievements.</returns>
        /// <seealso cref="GetAchievementAsync(IWolfClient, WolfLanguage, uint, CancellationToken)"/>
        /// <seealso cref="GetUserAchievementsAsync(IWolfClient, uint, WolfLanguage, CancellationToken)"/>
        /// <seealso cref="GetAllAchievementsAsync(IWolfClient, WolfLanguage, CancellationToken)"/>
        public static async Task<IEnumerable<WolfAchievement>> GetAchievementsAsync(this IWolfClient client, WolfLanguage language,
            IEnumerable<uint> achievementIDs, CancellationToken cancellationToken = default)
        {
            if (achievementIDs == null || !achievementIDs.Any())
                return await client.GetAllAchievementsAsync(language, cancellationToken).ConfigureAwait(false);

            // get as many achievements from cache as possible
            List<WolfAchievement> results = new List<WolfAchievement>(achievementIDs?.Count() ?? 600);
            if (client is IWolfClientCacheAccessor cache)
                results.AddRange(achievementIDs.Select(aID => cache.GetCachedAchievement(language, aID)).Where(a => a != null));

            // get the ones that aren't in cache from the server
            IEnumerable<uint> toRequest = achievementIDs?.Except(results.Select(a => a.ID));
            if (toRequest != null && toRequest.Any())
            {
                IEnumerable<WolfAchievement> allAchievements = await client.GetAllAchievementsAsync(language, cancellationToken).ConfigureAwait(false);
                results.AddRange(allAchievements?.Where(a => a != null && toRequest.Contains(a.ID)));
            }

            return results;
        }

        /// <summary>Retrieve achievement by ID.</summary>
        /// <remarks><para>Achievement already cached will be retrieved from cache.</para>
        /// <para>Due to the construction of the protocol, if achievement is not cached, the client will request all achievements again.</para></remarks>
        /// <param name="language">Language to retrieve achievements in.</param>
        /// <param name="id">ID of achievement to retrieve.</param>
        /// <returns>Retrieved achievement.</returns>
        /// <seealso cref="GetAchievementsAsync(IWolfClient, WolfLanguage, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetUserAchievementsAsync(IWolfClient, uint, WolfLanguage, CancellationToken)"/>
        /// <seealso cref="GetAllAchievementsAsync(IWolfClient, WolfLanguage, CancellationToken)"/>
        public static async Task<WolfAchievement> GetAchievementAsync(this IWolfClient client, WolfLanguage language, uint id, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfAchievement> result = await client.GetAchievementsAsync(language, new uint[] { id }, cancellationToken).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        /// <summary>Retrieve all achievements.</summary>
        /// <remarks><para>This method skips cache completely, and all achievements will be downloaded from the server.</para>
        /// <para>All child achievements will be surfaced to the top level, so can be accessed by direct enumerable queries.</para></remarks>
        /// <param name="language">Language to retrieve achievements in.</param>
        /// <returns>Enumerable of retrieved achievements.</returns>
        /// <seealso cref="GetAchievementAsync(IWolfClient, WolfLanguage, uint, CancellationToken)"/>
        /// <seealso cref="GetUserAchievementsAsync(IWolfClient, uint, WolfLanguage, CancellationToken)"/>
        /// <seealso cref="GetAchievementsAsync(IWolfClient, WolfLanguage, IEnumerable{uint}, CancellationToken)"/>
        public static async Task<IEnumerable<WolfAchievement>> GetAllAchievementsAsync(this IWolfClient client, WolfLanguage language, CancellationToken cancellationToken = default)
        {
            AchievementListResponse response = await client.SendAsync<AchievementListResponse>(
                new AchievementListMessage(language), cancellationToken).ConfigureAwait(false);
            return response.GetFlattenedAchievementList();
        }

        /// <summary>Retrieve user's achievements.</summary>
        /// <remarks><para>Achievements already cached will be retrieved from cache.</para>
        /// <para>Due to the construction of the protocol, if any achievement is not cached, the client will request all achievements again.</para>
        /// <para>All child achievements will be surfaced to the top level, so can be accessed by direct enumerable queries.</para></remarks>
        /// <param name="userID">ID of user to retrieve achievements of.</param>
        /// <param name="language">Language to retrieve achievements in.</param>
        /// <returns>Dictionary of user achievements, with keys being achievement and value being unlock time.</returns>
        /// <seealso cref="GetAchievementsAsync(IWolfClient, WolfLanguage, IEnumerable{uint}, CancellationToken)"/>
        /// <seealso cref="GetAchievementAsync(IWolfClient, WolfLanguage, uint, CancellationToken)"/>
        /// <seealso cref="GetAllAchievementsAsync(IWolfClient, WolfLanguage, CancellationToken)"/>
        public static async Task<IReadOnlyDictionary<WolfAchievement, DateTime>> GetUserAchievementsAsync(this IWolfClient client, uint userID, 
            WolfLanguage language, CancellationToken cancellationToken = default)
        {
            UserAchievementListResponse response = await client.SendAsync<UserAchievementListResponse>(
                new UserAchievementListMessage(userID), cancellationToken).ConfigureAwait(false);
            Dictionary<WolfAchievement, DateTime> results = new Dictionary<WolfAchievement, DateTime>(response?.UserAchievements?.Count ?? 0);
            if (response?.UserAchievements != null)
            {
                // get all achievements first
                IEnumerable<WolfAchievement> achivs =
                    await client.GetAchievementsAsync(language, response.UserAchievements.Keys, cancellationToken).ConfigureAwait(false);
                // map user achievements to retrieved achievement objects
                foreach (WolfAchievement a in achivs)
                    results.Add(a, response.UserAchievements[a.ID]);
            }
            return results;
        }
        #endregion


        /* ADMIN ACTIONS */
        #region Admin Actions
        /// <summary>Admin a group member.</summary>
        /// <param name="userID">User ID of group member to admin.</param>
        /// <param name="groupID">ID of group to admin the user in.</param>
        /// <seealso cref="ModUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ResetUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="SilenceUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="KickUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="BanUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        public static Task AdminUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Admin), cancellationToken);
        /// <summary>Mod a group member.</summary>
        /// <param name="userID">User ID of group member to mod.</param>
        /// <param name="groupID">ID of group to mod the user in.</param>
        /// <seealso cref="AdminUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ResetUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="SilenceUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="KickUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="BanUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        public static Task ModUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Mod), cancellationToken);
        /// <summary>Reset a group member.</summary>
        /// <param name="userID">User ID of group member to reset.</param>
        /// <param name="groupID">ID of group to reset the user in.</param>
        /// <seealso cref="AdminUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ModUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="SilenceUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="KickUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="BanUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        public static Task ResetUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.User), cancellationToken);
        /// <summary>Silence a group member.</summary>
        /// <param name="userID">User ID of group member to silence.</param>
        /// <param name="groupID">ID of group to silence the user in.</param>
        /// <seealso cref="AdminUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ModUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ResetUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="KickUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="BanUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        public static Task SilenceUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Silenced), cancellationToken);
        /// <summary>Kick a group member.</summary>
        /// <param name="userID">User ID of group member to kick.</param>
        /// <param name="groupID">ID of group to kick the user from.</param>
        /// <seealso cref="AdminUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ModUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ResetUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="SilenceUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="BanUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        public static Task KickUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.NotMember), cancellationToken);
        /// <summary>Ban a group member.</summary>
        /// <param name="userID">User ID of group member to ban.</param>
        /// <param name="groupID">ID of group to ban the user from.</param>
        /// <seealso cref="AdminUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ModUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="ResetUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="SilenceUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        /// <seealso cref="KickUserAsync(IWolfClient, uint, uint, CancellationToken)"/>
        public static Task BanUserAsync(this IWolfClient client, uint userID, uint groupID, CancellationToken cancellationToken = default)
            => client.SendAsync(new GroupAdminMessage(userID, groupID, WolfGroupCapabilities.Banned), cancellationToken);
        #endregion


        /* CHAT */
        #region Chat messages
        // sending
        /// <summary>Sends a private text message.</summary>
        /// <param name="userID">ID of user to send the message to.</param>
        /// <param name="text">Content of the message.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> SendPrivateTextMessageAsync(this IWolfClient client, uint userID, string text, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(userID, false, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text)), cancellationToken);
        /// <summary>Sends a group text message.</summary>
        /// <param name="groupID">ID of group to send the message to.</param>
        /// <param name="text">Content of the message.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> SendGroupTextMessageAsync(this IWolfClient client, uint groupID, string text, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(groupID, true, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text)), cancellationToken);


        /// <summary>Sends a private image message.</summary>
        /// <param name="userID">ID of user to send the message to.</param>
        /// <param name="imageBytes">Bytes of the image to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> SendPrivateImageMessageAsync(this IWolfClient client, uint userID, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(userID, false, ChatMessageTypes.Image, imageBytes), cancellationToken);
        /// <summary>Sends a group image message.</summary>
        /// <param name="groupID">ID of group to send the message to.</param>
        /// <param name="imageBytes">Bytes of the image to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> SendGroupImageMessageAsync(this IWolfClient client, uint groupID, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(groupID, true, ChatMessageTypes.Image, imageBytes), cancellationToken);

        /// <summary>Sends a private voice message.</summary>
        /// <param name="userID">ID of user to send the message to.</param>
        /// <param name="voiceBytes">Bytes of the voice to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> SendPrivateVoiceMessageAsync(this IWolfClient client, uint userID, IEnumerable<byte> voiceBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(userID, false, ChatMessageTypes.Voice, voiceBytes), cancellationToken);
        /// <summary>Sends a group voice message.</summary>
        /// <param name="groupID">ID of group to send the message to.</param>
        /// <param name="voiceBytes">Bytes of the voice to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> SendGroupVoiceMessageAsync(this IWolfClient client, uint groupID, IEnumerable<byte> voiceBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(groupID, true, ChatMessageTypes.Voice, voiceBytes), cancellationToken);

        // responding
        /// <summary>Sends a text message response message to group or user.</summary>
        /// <param name="incomingMessage">Message the user or group sent to the client.</param>
        /// <param name="text">Content of the message.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> ReplyTextAsync(this IWolfClient client, ChatMessage incomingMessage, string text, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(incomingMessage.IsGroupMessage ? incomingMessage.RecipientID : incomingMessage.SenderID.Value,
                incomingMessage.IsGroupMessage, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text)), cancellationToken);
        /// <summary>Sends an image response message to group or user.</summary>
        /// <param name="incomingMessage">Message the user or group sent to the client.</param>
        /// <param name="imageBytes">Bytes of the image to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> ReplyImageAsync(this IWolfClient client, ChatMessage incomingMessage, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(incomingMessage.IsGroupMessage ? incomingMessage.RecipientID : incomingMessage.SenderID.Value,
                incomingMessage.IsGroupMessage, ChatMessageTypes.Image, imageBytes), cancellationToken);
        /// <summary>Sends a voice response message to group or user.</summary>
        /// <param name="incomingMessage">Message the user or group sent to the client.</param>
        /// <param name="voiceBytes">Bytes of the voice to send.</param>
        /// <returns>Message sending response.</returns>
        public static Task<ChatResponse> ReplyVoiceAsync(this IWolfClient client, ChatMessage incomingMessage, IEnumerable<byte> voiceBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(incomingMessage.IsGroupMessage ? incomingMessage.RecipientID : incomingMessage.SenderID.Value,
                incomingMessage.IsGroupMessage, ChatMessageTypes.Voice, voiceBytes), cancellationToken);
        #endregion
    }
}
