using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Messages.Serialization.Internal;
using TehGM.Wolfringo.Socket;

namespace TehGM.Wolfringo.Caching.Internal
{
    /// <summary>Utility grouping common entity caches together.</summary>
    /// <remarks><para>This utility contains caches for entities that Wolf client is caching.</para>
    /// <para>Each cache can be separately enabled or disabled.</para></remarks>
    public class WolfClientCache : IWolfClientCache, IDisposable
    {
        /// <summary>Whether caching of <see cref="WolfUser"/> is enabled.</summary>
        public bool IsUsersCachingEnabled { get; }
        /// <summary>Whether caching of <see cref="WolfGroup"/> is enabled.</summary>
        public bool IsGroupsCachingEnabled { get; }
        /// <summary>Whether caching of <see cref="WolfCharm"/> is enabled.</summary>
        public bool IsCharmsCachingEnabled { get; }
        /// <summary>Whether caching of <see cref="WolfAchievement"/> is enabled.</summary>
        public bool IsAchievementsCachingEnabled { get; }

        /// <summary>Users cache.</summary>
        protected IWolfCachedEntityCollection<WolfUser> UsersCache { get; }
        /// <summary>Groups cache.</summary>
        protected IWolfCachedEntityCollection<WolfGroup> GroupsCache { get; }
        /// <summary>Charms cache.</summary>
        protected IWolfCachedEntityCollection<WolfCharm> CharmsCache { get; }
        /// <summary>Achievements cache.</summary>
        protected IWolfCachedEntityCollection<WolfLanguage, WolfAchievement> AchievementsCache { get; }
        /// <summary>A logger for logging messages.</summary>
        protected ILogger Log { get; }

        /// <summary>Creates new container and contained caches with all caches enabled.</summary>
        /// <remarks>All caches will be enabled by default.</remarks>
        /// <param name="log">Logger that this cache can log to. If null, logging will be disabled.</param>
        public WolfClientCache(ILogger log = null)
            : this(new WolfCacheOptions(), log) { }

        /// <summary>Creates new container and contained caches.</summary>
        /// <param name="log">Logger that this cache can log to. If null, logging will be disabled.</param>
        /// <param name="options">Options specifying which caches should be enabled.</param>
        public WolfClientCache(WolfCacheOptions options, ILogger<WolfClientCache> log)
            : this(options, (ILogger)log) { }

        /// <summary>Creates new container and contained caches.</summary>
        /// <param name="log">Logger that this cache can log to. If null, logging will be disabled.</param>
        /// <param name="options">Options specifying which caches should be enabled.</param>
        public WolfClientCache(WolfCacheOptions options, ILogger log = null)
        {
            // init caches
            this.UsersCache = new WolfCachedEntityCollection<WolfUser>();
            this.GroupsCache = new WolfCachedEntityCollection<WolfGroup>();
            this.CharmsCache = new WolfCachedEntityCollection<WolfCharm>();
            this.AchievementsCache = new WolfCachedEntityCollection<WolfLanguage, WolfAchievement>();

            // enable according to options
            this.IsUsersCachingEnabled = options.UsersCachingEnabled;
            this.IsGroupsCachingEnabled = options.GroupsCachingEnabled;
            this.IsCharmsCachingEnabled = options.CharmsCachingEnabled;
            this.IsAchievementsCachingEnabled = options.AchievementsCachingEnabled;

            // assign log
            this.Log = log;
        }

        /// <inheritdoc/>
        public WolfUser GetCachedUser(uint id)
        {
            if (!this.IsUsersCachingEnabled)
                return null;
            WolfUser result = this.UsersCache?.Get(id);
            if (result != null)
                this.Log?.LogTrace("User {UserID} found in cache", id);
            return result;
        }

        /// <inheritdoc/>
        public WolfGroup GetCachedGroup(uint id)
        {
            if (!this.IsGroupsCachingEnabled)
                return null;
            WolfGroup result = this.GroupsCache?.Get(id);
            if (result != null)
                this.Log?.LogTrace("Group {GroupID} found in cache", id);
            return result;
        }

        /// <inheritdoc/>
        public WolfGroup GetCachedGroup(string name)
        {
            if (!this.IsGroupsCachingEnabled)
                return null;
            WolfGroup result = this.GroupsCache?.Find(group => group.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (result != null)
                this.Log?.LogTrace("Group {GroupName} found in cache", name);
            return result;
        }

        /// <inheritdoc/>
        public WolfCharm GetCachedCharm(uint id)
        {
            if (!this.IsCharmsCachingEnabled)
                return null;
            WolfCharm result = this.CharmsCache?.Get(id);
            if (result != null)
                this.Log?.LogTrace("Charm {CharmID} found in cache", id);
            return result;
        }

        /// <inheritdoc/>
        public WolfAchievement GetCachedAchievement(WolfLanguage language, uint id)
        {
            if (!this.IsAchievementsCachingEnabled)
                return null;
            WolfAchievement result = this.AchievementsCache?.Get(language, id);
            if (result != null)
                this.Log?.LogTrace("Achievement {AchievementID} found in cache", id);
            return result;
        }

        /// <summary>Clears all caches.</summary>
        public virtual void Clear()
        {
            this.Log?.LogTrace("Clearing Wolf Client caches");
            this.UsersCache?.Clear();
            this.GroupsCache?.Clear();
            this.CharmsCache?.Clear();
            this.AchievementsCache?.ClearAll();
        }

        /// <summary>Logs message with Warning level, and returns true.</summary>
        /// <remarks>Designed to use with `catch (Exception) when` pattern. Preserves log scope for logged message.</remarks>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>True.</returns>
        protected bool LogWarning(string message, params object[] args)
        {
            this.Log?.LogWarning(message, args);
            return true;
        }

        void IDisposable.Dispose()
            => this.Clear();

        /// <inheritdoc/>
        public virtual Task OnConnectingAsync(IWolfClient client, CancellationToken cancellationToken = default)
        {
            this.Clear();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual void OnDisconnected(IWolfClient client, SocketClosedEventArgs e)
            => this.Clear();

        #region MESSAGE READING
        /// <inheritdoc/>
        public virtual Task OnMessageSentAsync(IWolfClient client, IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default)
        {
            if (this.IsUsersCachingEnabled)
            {
                // update users cache if it's user profile message
                if (response is UserProfileResponse userProfileResponse && userProfileResponse.UserProfiles?.Any() == true)
                {
                    foreach (WolfUser user in userProfileResponse.UserProfiles)
                    {
                        if (user == null)
                            continue;

                        this.UsersCache?.AddOrReplaceIfChanged(user);
                    }
                }
            }

            if (this.IsGroupsCachingEnabled)
            {
                // update groups cache if it's group profile message
                if (response is GroupProfileResponse groupProfileResponse && groupProfileResponse.GroupProfiles?.Any() == true)
                {
                    foreach (WolfGroup group in groupProfileResponse.GroupProfiles)
                    {
                        if (group == null)
                            continue;

                        // repopulate group members if new group profile came without them
                        WolfGroup existingGroup = this.GroupsCache?.Get(group.ID);
                        if (existingGroup != null && existingGroup.Members?.Any() == true && group.Members?.Any() != true)
                            EntityModificationHelper.ReplaceAllGroupMembers(group, existingGroup.Members.Values);
                        // replace cached group itself
                        this.GroupsCache?.AddOrReplaceIfChanged(group);
                    }
                }

                // add group if it was created
                else if (response is GroupEditResponse groupEditResponse && message is GroupCreateMessage)
                    this.GroupsCache.AddOrReplaceIfChanged(groupEditResponse.GroupProfile);

                // update group audio config
                else if (message is GroupAudioUpdateResponse groupAudioUpdateResponse && groupAudioUpdateResponse.AudioConfig != null)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupAudioUpdateResponse.AudioConfig.GroupID);
                    if (cachedGroup != null)
                    {
                        this.Log?.LogTrace("Updating cached group {GroupID} audio config", cachedGroup.ID);
                        rawResponse.Payload.PopulateObject(cachedGroup.AudioConfig, "body");
                    }
                }
            }


            if (this.IsCharmsCachingEnabled)
            {
                // update charms cache if it's charms list
                if (response is CharmListResponse listCharmsResponse && listCharmsResponse.Charms?.Any() == true)
                {
                    foreach (WolfCharm charm in listCharmsResponse.Charms)
                        this.CharmsCache?.AddOrReplace(charm);
                }
            }

            if (this.IsAchievementsCachingEnabled)
            {
                if (response is AchievementListResponse achievementListResponse && achievementListResponse.Achievements?.Any() == true &&
                    message is AchievementListMessage achievementListMessage)
                {
                    foreach (WolfAchievement achiv in achievementListResponse.GetFlattenedAchievementList())
                        this.AchievementsCache?.AddOrReplace(achievementListMessage.Language, achiv);
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task OnMessageReceivedAsync(IWolfClient client, IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default)
        {
            // update user presence
            if (this.IsUsersCachingEnabled)
            {
                if (message is PresenceUpdateEvent presenceUpdate)
                {
                    WolfUser cachedUser = this.UsersCache?.Get(presenceUpdate.UserID);
                    if (cachedUser != null)
                    {
                        this.Log?.LogTrace("Updating cached user {UserID} presence", cachedUser.ID);
                        rawMessage.Payload.PopulateObject(cachedUser, "body");
                    }
                }
                else if (message is UserUpdateEvent userUpdatedEvent)
                {
                    WolfUser cachedUser = this.UsersCache?.Get(userUpdatedEvent.UserID);
                    if (cachedUser == null || string.IsNullOrWhiteSpace(userUpdatedEvent.Hash) || cachedUser.Hash != userUpdatedEvent.Hash)
                    {
                        this.Log?.LogTrace("Updating user {UserID}", userUpdatedEvent.UserID);
                        await client.SendAsync<UserProfileResponse>(
                            new UserProfileMessage(new uint[] { userUpdatedEvent.UserID }, true, true),
                            cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            if (this.IsGroupsCachingEnabled)
            {
                // update group audio count
                if (message is GroupAudioCountUpdateEvent groupAudioCountUpdate)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupAudioCountUpdate.GroupID);
                    if (cachedGroup != null)
                    {
                        this.Log?.LogTrace("Updating cached group {GroupID} audio counts", cachedGroup.ID);
                        rawMessage.Payload.PopulateObject(cachedGroup.AudioCounts, "body");
                    }
                }

                // update group audio config
                if (message is GroupAudioUpdateMessage groupAudioUpdate)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupAudioUpdate.GroupID);
                    if (cachedGroup != null)
                    {
                        this.Log?.LogTrace("Updating cached group {GroupID} audio config", cachedGroup.ID);
                        rawMessage.Payload.PopulateObject(cachedGroup.AudioConfig, "body");
                    }
                }

                // update group when change event by requesting group profile
                else if (message is GroupUpdateEvent groupUpdate)
                {
                    // trigger group download only if cached group has different hash
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupUpdate.GroupID);
                    if (cachedGroup != null && cachedGroup.Hash != groupUpdate.Hash)
                    {
                        await client.SendAsync<GroupProfileResponse>(
                            new GroupProfileMessage(new uint[] { groupUpdate.GroupID }), cancellationToken).ConfigureAwait(false);
                    }
                }

                // update group member list when one joined
                else if (message is GroupJoinMessage groupMemberJoined)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupMemberJoined.GroupID.Value);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.SetGroupMember(cachedGroup,
                                new WolfGroupMember(groupMemberJoined.UserID.Value, groupMemberJoined.Capabilities.Value));
                    }
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
                }

                // update group member list if one left
                else if (message is GroupLeaveMessage groupMemberLeft)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupMemberLeft.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.RemoveGroupMember(cachedGroup, groupMemberLeft.UserID.Value);
                    }
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
                }

                // update group member capabilities if member was updated
                else if (message is GroupMemberUpdateEvent groupMemberUpdated)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupMemberUpdated.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.SetGroupMember(cachedGroup,
                                new WolfGroupMember(groupMemberUpdated.UserID, groupMemberUpdated.Capabilities));
                    }
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
                }
            }
        }
        #endregion
    }
}
