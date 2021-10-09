using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Utility grouping common entity caches together.</summary>
    /// <remarks><para>This utility contains caches for entities that Wolf client is caching.</para>
    /// <para>Each cache can be separately enabled or disabled.</para></remarks>
    public class WolfEntityCacheContainer : IWolfClientCache
    {
        private bool _enableUsersCaching;
        private bool _enableGroupsCaching;
        private bool _enableCharmsCaching;
        private bool _enableAchievementsCaching;

        /// <summary>Users cache.</summary>
        public IWolfEntityCache<WolfUser> UsersCache { get; }
        /// <summary>Groups cache.</summary>
        public IWolfEntityCache<WolfGroup> GroupsCache { get; }
        /// <summary>Charms cache.</summary>
        public IWolfEntityCache<WolfCharm> CharmsCache { get; }
        /// <summary>Achievements cache.</summary>
        public IWolfEntityCache<WolfLanguage, WolfAchievement> AchievementsCache { get; }

        private readonly ILogger _log;

        /// <summary>Creates new container and contained caches with all caches enabled.</summary>
        /// <remarks>All caches will be enabled by default.</remarks>
        /// <param name="log">Logger that this cache can log to. If null, logging will be disabled.</param>
        public WolfEntityCacheContainer(ILogger log = null)
            : this(new WolfCacheOptions(), log) { }

        /// <summary>Creates new container and contained caches.</summary>
        /// <param name="log">Logger that this cache can log to. If null, logging will be disabled.</param>
        /// <param name="options">Options specifying which caches should be enabled.</param>
        public WolfEntityCacheContainer(WolfCacheOptions options, ILogger<WolfEntityCacheContainer> log)
            : this(options, (ILogger)log) { }

        /// <summary>Creates new container and contained caches.</summary>
        /// <param name="log">Logger that this cache can log to. If null, logging will be disabled.</param>
        /// <param name="options">Options specifying which caches should be enabled.</param>
        public WolfEntityCacheContainer(WolfCacheOptions options, ILogger log = null)
        {
            // init caches
            this.UsersCache = new WolfEntityCache<WolfUser>();
            this.GroupsCache = new WolfEntityCache<WolfGroup>();
            this.CharmsCache = new WolfEntityCache<WolfCharm>();
            this.AchievementsCache = new WolfEntityCache<WolfLanguage, WolfAchievement>();

            // enable according to options
            this._enableUsersCaching = options.UsersCachingEnabled;
            this._enableGroupsCaching = options.GroupsCachingEnabled;
            this._enableCharmsCaching = options.CharmsCachingEnabled;
            this._enableAchievementsCaching = options.AchievementsCachingEnabled;

            // assign log
            this._log = log;
        }

        /// <summary>Clears all caches.</summary>
        public virtual void ClearAll()
        {
            this.UsersCache?.Clear();
            this.GroupsCache?.Clear();
            this.CharmsCache?.Clear();
            this.AchievementsCache?.ClearAll();
        }

        /// <inheritdoc/>
        public Task HandleMessageSentAsync(IWolfClient client, IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default)
        {
            if (this._enableUsersCaching)
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

            if (this._enableGroupsCaching)
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

                // update group member list if one was requested
                else if (response is GroupMembersListResponse groupMembersResponse && message is GroupMembersListMessage groupMembersMessage && groupMembersResponse.GroupMembers?.Any() == true)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupMembersMessage.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.ReplaceAllGroupMembers(cachedGroup, groupMembersResponse.GroupMembers);
                    }
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
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
                        this._log?.LogTrace("Updating cached group {GroupID} audio config", cachedGroup.ID);
                        rawResponse.Payload.PopulateObject(cachedGroup.AudioConfig, "body");
                    }
                }
            }


            if (this._enableCharmsCaching)
            {
                // update charms cache if it's charms list
                if (response is CharmListResponse listCharmsResponse && listCharmsResponse.Charms?.Any() == true)
                {
                    foreach (WolfCharm charm in listCharmsResponse.Charms)
                        this.CharmsCache?.AddOrReplace(charm);
                }
            }

            if (this._enableAchievementsCaching)
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
        public async Task HandleMessageReceivedAsync(IWolfClient client, IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default)
        {
            // update user presence
            if (this._enableUsersCaching)
            {
                if (message is PresenceUpdateEvent presenceUpdate)
                {
                    WolfUser cachedUser = this.UsersCache?.Get(presenceUpdate.UserID);
                    if (cachedUser != null)
                    {
                        this._log?.LogTrace("Updating cached user {UserID} presence", cachedUser.ID);
                        rawMessage.Payload.PopulateObject(cachedUser, "body");
                    }
                }
                else if (message is UserUpdateEvent userUpdatedEvent)
                {
                    WolfUser cachedUser = this.UsersCache?.Get(userUpdatedEvent.UserID);
                    if (cachedUser == null || string.IsNullOrWhiteSpace(userUpdatedEvent.Hash) || cachedUser.Hash != userUpdatedEvent.Hash)
                    {
                        this._log?.LogTrace("Updating user {UserID}", userUpdatedEvent.UserID);
                        await client.SendAsync<UserProfileResponse>(
                            new UserProfileMessage(new uint[] { userUpdatedEvent.UserID }, true, true),
                            cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            if (this._enableGroupsCaching)
            {
                // update group audio count
                if (message is GroupAudioCountUpdateEvent groupAudioCountUpdate)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupAudioCountUpdate.GroupID);
                    if (cachedGroup != null)
                    {
                        this._log?.LogTrace("Updating cached group {GroupID} audio counts", cachedGroup.ID);
                        rawMessage.Payload.PopulateObject(cachedGroup.AudioCounts, "body");
                    }
                }

                // update group audio config
                if (message is GroupAudioUpdateMessage groupAudioUpdate)
                {
                    WolfGroup cachedGroup = this.GroupsCache?.Get(groupAudioUpdate.GroupID);
                    if (cachedGroup != null)
                    {
                        this._log?.LogTrace("Updating cached group {GroupID} audio config", cachedGroup.ID);
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

        private bool LogWarning(string message, params object[] args)
        {
            this._log?.LogWarning(message, args);
            return true;
        }
    }
}
