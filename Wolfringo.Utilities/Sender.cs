using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            NotificationsListResponse response = await client.SendAsync<NotificationsListResponse>(new NotificationsListMessage(language, device), cancellationToken).ConfigureAwait(false);
            return response.Notifications?.Any() == true ? response.Notifications : Enumerable.Empty<WolfNotification>();
        }
        public static Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.GetNotificationsAsync(WolfLanguage.English, WolfDevice.Bot, cancellationToken);

        public static Task ClearNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.SendAsync(new NotificationsClearMessage(), cancellationToken);
        #endregion


        /** USERS **/
        #region Users
        // retrieving
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

        // contacts
        public static async Task<IEnumerable<WolfUser>> GetContactListAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            ContactListResponse response = await client.SendAsync<ContactListResponse>(new ContactListMessage(), cancellationToken).ConfigureAwait(false);
            return await client.GetUsersAsync(response.ContactIDs, cancellationToken).ConfigureAwait(false);
        }

        public static Task AddContactAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new ContactAddMessage(userID), cancellationToken);

        public static Task DeleteContactAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new ContactDeleteMessage(userID), cancellationToken);

        // blocking
        public static async Task<IEnumerable<WolfUser>> GetBlockedUsersAsync(this IWolfClient client, CancellationToken cancellationToken = default)
        {
            BlockListResponse response = await client.SendAsync<BlockListResponse>(
                new BlockListMessage(), cancellationToken).ConfigureAwait(false);
            if (response.BlockedUsersIDs?.Any() != true)
                return Enumerable.Empty<WolfUser>();
            return await client.GetUsersAsync(response.BlockedUsersIDs, cancellationToken).ConfigureAwait(false);
        }

        public static Task BlockUserAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new BlockAddMessage(userID), cancellationToken);

        public static Task UnblockUserAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync(new BlockDeleteMessage(userID), cancellationToken);

        // updating
        public static async Task<WolfUser> UpdateProfileAsync(this IWolfClient client, Action<UserUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            WolfUser currentUser = await client.GetCurrentUserAsync(cancellationToken).ConfigureAwait(false);
            UserUpdateMessage.Builder updateBuilder = new UserUpdateMessage.Builder(currentUser);
            updates?.Invoke(updateBuilder);
            UserUpdateResponse response = await client.SendAsync<UserUpdateResponse>(updateBuilder.Build(), cancellationToken).ConfigureAwait(false);
            return response.UserProfile;
        }

        public static Task<WolfUser> UpdateNicknameAsync(this IWolfClient client, string newNickname, CancellationToken cancellationToken = default)
            => client.UpdateProfileAsync(user => user.Nickname = newNickname, cancellationToken);

        public static Task<WolfUser> UpdateStatusAsync(this IWolfClient client, string newStatus, CancellationToken cancellationToken = default)
            => client.UpdateProfileAsync(user => user.Status = newStatus, cancellationToken);
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
                        GroupMembersListResponse membersResponse = await client.SendAsync<GroupMembersListResponse>(
                            new GroupMembersListMessage(group.ID), cancellationToken).ConfigureAwait(false);
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
            GroupListResponse response = await client.SendAsync<GroupListResponse>(
                new GroupListMessage(), cancellationToken).ConfigureAwait(false);
            return await client.GetGroupsAsync(response.UserGroupIDs, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<WolfGroupStatistics> GetGroupStatisticsAsync(this IWolfClient client, uint groupID, CancellationToken cancellationToken = default)
        {
            GroupStatisticsResponse response = await client.SendAsync<GroupStatisticsResponse>(
                new GroupStatisticsMessage(groupID), cancellationToken).ConfigureAwait(false);
            return response.GroupStatistics;
        }

        // create
        public static async Task<WolfGroup> CreateGroupAsync(this IWolfClient client, string groupName, string groupDescription, Action<GroupCreateMessage.Builder> groupSettings, CancellationToken cancellationToken = default)
        {
            GroupCreateMessage.Builder builder = new GroupCreateMessage.Builder(groupName, groupDescription);
            groupSettings?.Invoke(builder);
            GroupEditResponse response = await client.SendAsync<GroupEditResponse>(builder.Build(), cancellationToken).ConfigureAwait(false);
            return response.GroupProfile;
        }

        public static Task<WolfGroup> CreateGroupAsync(this IWolfClient client, string groupName, string groupDescription, CancellationToken cancellationToken = default)
            => client.CreateGroupAsync(groupName, groupDescription, null, cancellationToken);

        // update
        public static async Task<WolfGroup> UpdateGroupAsync(this IWolfClient client, WolfGroup group,
            Action<GroupUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            GroupUpdateMessage.Builder builder = new GroupUpdateMessage.Builder(group);
            updates?.Invoke(builder);
            GroupEditResponse response = await client.SendAsync<GroupEditResponse>(builder.Build(), cancellationToken).ConfigureAwait(false);
            return response.GroupProfile;
        }

        public static async Task<WolfGroup> UpdateGroupAsync(this IWolfClient client, uint groupID,
            Action<GroupUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            WolfGroup group = await client.GetGroupAsync(groupID, cancellationToken).ConfigureAwait(false);
            return await client.UpdateGroupAsync(group, updates, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<WolfGroup.WolfGroupAudioConfig> UpdateGroupAudioConfigAsync(this IWolfClient client, WolfGroup group,
            Action<GroupAudioUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            GroupAudioUpdateMessage.Builder builder = new GroupAudioUpdateMessage.Builder(group.AudioConfig);
            updates?.Invoke(builder);
            GroupAudioUpdateResponse response = await client.SendAsync<GroupAudioUpdateResponse>(
                builder.Build(), cancellationToken).ConfigureAwait(false);
            return response.AudioConfig;
        }

        public static async Task<WolfGroup.WolfGroupAudioConfig> UpdateGroupAudioConfigAsync(this IWolfClient client, uint groupID,
            Action<GroupAudioUpdateMessage.Builder> updates, CancellationToken cancellationToken = default)
        {
            WolfGroup group = await client.GetGroupAsync(groupID, cancellationToken).ConfigureAwait(false);
            return await client.UpdateGroupAudioConfigAsync(group, updates, cancellationToken).ConfigureAwait(false);
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
                CharmListResponse response = await client.SendAsync<CharmListResponse>(
                    new CharmListMessage(toRequest), cancellationToken).ConfigureAwait(false);
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


        // charm ownership
        public static Task<CharmStatisticsResponse> GetUserCharmStatsAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
            => client.SendAsync<CharmStatisticsResponse>(new CharmStatisticsMessage(userID), cancellationToken);

        public static async Task<WolfCharm> GetUserCurrentCharm(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            WolfUser user = await client.GetUserAsync(userID, cancellationToken).ConfigureAwait(false);
            if (user == null)
                throw new KeyNotFoundException($"User {userID} not found");
            if (user.ActiveCharmID == null)
                return null;
            return await client.GetCharmAsync(user.ActiveCharmID.Value, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<WolfCharmSubscription>> GetUserActiveCharmsAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            UserCharmsListResponse response = await client.SendAsync<UserCharmsListResponse>(
                new UserActiveCharmsListMessage(userID), cancellationToken).ConfigureAwait(false);
            return response.Charms;
        } public static async Task<IEnumerable<WolfCharmSubscription>> GetUserExpiredCharmsAsync(this IWolfClient client, uint userID, CancellationToken cancellationToken = default)
        {
            UserCharmsListResponse response = await client.SendAsync<UserCharmsListResponse>(
                new UserExpiredCharmsListMessage(userID), cancellationToken).ConfigureAwait(false);
            return response.Charms;
        }

        // setting charm
        public static Task SetActiveCharmAsync(this IWolfClient client, uint charmID, CancellationToken cancellationToken = default)
            => client.SendAsync(new UserCharmsSelectMessage(new Dictionary<int, uint>() { { 0, charmID } }), cancellationToken);
        public static Task RemoveActiveCharmAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.SendAsync(new UserCharmsSelectMessage(new Dictionary<int, uint>()), cancellationToken);
        #endregion


        /** ACHIEVEMENTS **/
        #region Achievements
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

        public static async Task<WolfAchievement> GetAchievementAsync(this IWolfClient client, WolfLanguage language, uint id, CancellationToken cancellationToken = default)
        {
            IEnumerable<WolfAchievement> result = await client.GetAchievementsAsync(language, new uint[] { id }, cancellationToken).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public static async Task<IEnumerable<WolfAchievement>> GetAllAchievementsAsync(this IWolfClient client, WolfLanguage language, CancellationToken cancellationToken = default)
        {
            AchievementListResponse response = await client.SendAsync<AchievementListResponse>(
                new AchievementListMessage(language), cancellationToken).ConfigureAwait(false);
            return response.GetFlattenedAchievementList();
        }

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


        /** CHAT **/
        #region Chat messages
        // sending
        public static Task<ChatResponse> SendPrivateTextMessageAsync(this IWolfClient client, uint userID, string text, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(userID, false, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text)), cancellationToken);
        public static Task<ChatResponse> SendGroupTextMessageAsync(this IWolfClient client, uint groupID, string text, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(groupID, true, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text)), cancellationToken);

        public static Task<ChatResponse> SendPrivateImageMessageAsync(this IWolfClient client, uint userID, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(userID, false, ChatMessageTypes.Image, imageBytes), cancellationToken);
        public static Task<ChatResponse> SendGroupImageMessageAsync(this IWolfClient client, uint groupID, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(groupID, true, ChatMessageTypes.Image, imageBytes), cancellationToken);

        public static Task<ChatResponse> SendPrivateVoiceMessageAsync(this IWolfClient client, uint userID, IEnumerable<byte> voiceBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(userID, false, ChatMessageTypes.Voice, voiceBytes), cancellationToken);
        public static Task<ChatResponse> SendGroupVoiceMessageAsync(this IWolfClient client, uint groupID, IEnumerable<byte> voiceBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(groupID, true, ChatMessageTypes.Voice, voiceBytes), cancellationToken);

        // responding
        public static Task<ChatResponse> RespondWithTextAsync(this IWolfClient client, ChatMessage incomingMessage, string text, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(
                incomingMessage.SenderID.Value, incomingMessage.IsGroupMessage, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text)), cancellationToken);
        public static Task<ChatResponse> RespondWithImageAsync(this IWolfClient client, ChatMessage incomingMessage, IEnumerable<byte> imageBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(
                incomingMessage.SenderID.Value, incomingMessage.IsGroupMessage, ChatMessageTypes.Image, imageBytes), cancellationToken);
        public static Task<ChatResponse> RespondWithVoiceAsync(this IWolfClient client, ChatMessage incomingMessage, IEnumerable<byte> voiceBytes, CancellationToken cancellationToken = default)
            => client.SendAsync<ChatResponse>(new ChatMessage(
                incomingMessage.SenderID.Value, incomingMessage.IsGroupMessage, ChatMessageTypes.Voice, voiceBytes), cancellationToken);
        #endregion
    }
}
