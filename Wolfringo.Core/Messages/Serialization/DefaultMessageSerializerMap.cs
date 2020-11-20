using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Default message command to message serializer map.</summary>
    /// <remarks><para>This class contains all Wolfringo library default message mappings, and will be used by default clients
    /// if no other map is provided.</para>
    /// <para>This class can be easily extended without inheritance. To provide custom mappings, call <see cref="MapSerializer(string, IMessageSerializer)"/>, 
    /// or pass custom mappings dictionary via the constructor. However, usage with .NET Core Host might require inheriting 
    /// and registering as a service in the service container - in this case, simply call <see cref="MapSerializer(string, IMessageSerializer)"/>
    /// in child class constructor for each custom mapping that needs to be made.</para></remarks>
    public class DefaultMessageSerializerMap : ISerializerMap<string, IMessageSerializer>
    {
        /// <summary>Fallback serializer that can be used if key has no mapped serializer. 
        /// Note that this serializer cannot be used for deserialization, and will be used only for serialization.</summary>
        public IMessageSerializer FallbackSerializer { get; set; }

        private IDictionary<string, IMessageSerializer> _map;

        /// <summary>Creates default message serializer map.</summary>
        /// <param name="fallbackSerializer">Serializer to use as fallback. If null, 
        /// <see cref="DefaultMessageSerializer{T}"/> for <see cref="IWolfMessage"/> will be used.</param>
        public DefaultMessageSerializerMap(IMessageSerializer fallbackSerializer = null)
        {
            this.FallbackSerializer = fallbackSerializer ?? new DefaultMessageSerializer<IWolfMessage>();
            this._map = new Dictionary<string, IMessageSerializer>(StringComparer.OrdinalIgnoreCase)
            {
                // default ones
                { MessageEventNames.Welcome, new DefaultMessageSerializer<WelcomeEvent>() },
                { MessageEventNames.SecurityLogin, new DefaultMessageSerializer<LoginMessage>() },
                { MessageEventNames.MessagePrivateSubscribe, new DefaultMessageSerializer<SubscribeToPmMessage>() },
                { MessageEventNames.MessageGroupSubscribe, new DefaultMessageSerializer<SubscribeToGroupMessage>() },
                { MessageEventNames.NotificationList, new DefaultMessageSerializer<NotificationsListMessage>() },
                { MessageEventNames.SubscriberProfile, new DefaultMessageSerializer<UserProfileMessage>() },
                { MessageEventNames.SubscriberContactList, new DefaultMessageSerializer<ContactListMessage>() },
                { MessageEventNames.PresenceUpdate, new DefaultMessageSerializer<PresenceUpdateEvent>() },
                { MessageEventNames.SubscriberSettingsUpdate, new DefaultMessageSerializer<OnlineStateUpdateMessage>() },
                { MessageEventNames.SecurityLogout, new DefaultMessageSerializer<LogoutMessage>() },
                { MessageEventNames.GroupProfile, new DefaultMessageSerializer<GroupProfileMessage>() },
                { MessageEventNames.GroupAudioCountUpdate, new DefaultMessageSerializer<GroupAudioCountUpdateEvent>() },
                { MessageEventNames.GroupUpdate, new DefaultMessageSerializer<GroupUpdateEvent>() },
                { MessageEventNames.GroupMemberList, new DefaultMessageSerializer<GroupMembersListMessage>() },
                { MessageEventNames.MessageGroupHistoryList, new DefaultMessageSerializer<GroupChatHistoryMessage>() },
                { MessageEventNames.MessagePrivateHistoryList, new DefaultMessageSerializer<PrivateChatHistoryMessage>() },
                { MessageEventNames.MessageConversationList, new DefaultMessageSerializer<RecentConversationsMessage>() },
                { MessageEventNames.SubscriberGroupList, new DefaultMessageSerializer<GroupListMessage>() },
                { MessageEventNames.CharmList, new DefaultMessageSerializer<CharmListMessage>() },
                { MessageEventNames.CharmSubscriberStatistics, new DefaultMessageSerializer<CharmStatisticsMessage>() },
                { MessageEventNames.GroupMemberUpdate, new DefaultMessageSerializer<GroupMemberUpdateEvent>() },
                { MessageEventNames.GroupAdmin, new DefaultMessageSerializer<GroupAdminMessage>() },
                { MessageEventNames.SubscriberUpdate, new DefaultMessageSerializer<UserUpdateEvent>() },
                { MessageEventNames.NotificationListClear, new DefaultMessageSerializer<NotificationsClearMessage>() },
                { MessageEventNames.MessageGroupUnsubscribe, new DefaultMessageSerializer<UnsubscribeFromGroupMessage>() },
                { MessageEventNames.MessagePrivateUnsubscribe, new DefaultMessageSerializer<UnsubscribeFromPrivateMessage>() },
                { MessageEventNames.SubscriberBlockList, new DefaultMessageSerializer<BlockListMessage>() },
                { MessageEventNames.SubscriberBlockAdd, new DefaultMessageSerializer<BlockAddMessage>() },
                { MessageEventNames.SubscriberBlockDelete, new DefaultMessageSerializer<BlockDeleteMessage>() },
                { MessageEventNames.GroupAudioUpdate, new DefaultMessageSerializer<GroupAudioUpdateMessage>() },
                { MessageEventNames.AchievementList, new DefaultMessageSerializer<AchievementListMessage>() },
                { MessageEventNames.AchievementSubscriberList, new DefaultMessageSerializer<UserAchievementListMessage>() },
                { MessageEventNames.GroupStats, new DefaultMessageSerializer<GroupStatisticsMessage>() },
                { MessageEventNames.CharmSubscriberExpiredList, new DefaultMessageSerializer<UserExpiredCharmsListMessage>() },
                { MessageEventNames.CharmSubscriberActiveList, new DefaultMessageSerializer<UserActiveCharmsListMessage>() },
                { MessageEventNames.CharmSubscriberSetSelected, new DefaultMessageSerializer<UserCharmsSelectMessage>() },
                { MessageEventNames.TipGroupSubscribe, new DefaultMessageSerializer<SubscribeToGroupTipsMessage>() },
                { MessageEventNames.TipSummary, new DefaultMessageSerializer<TipSummaryMessage>() },
                { MessageEventNames.TipDetail, new DefaultMessageSerializer<TipDetailsMessage>() },
                // group join and leave
                { MessageEventNames.GroupMemberAdd, new GroupJoinLeaveMessageSerializer<GroupJoinMessage>() },
                { MessageEventNames.GroupMemberDelete, new GroupJoinLeaveMessageSerializer<GroupLeaveMessage>() },
                // contact add and delete
                { MessageEventNames.SubscriberContactAdd, new ContactAddDeleteMessageSerializer<ContactAddMessage>() },
                { MessageEventNames.SubscriberContactDelete, new ContactAddDeleteMessageSerializer<ContactDeleteMessage>() },
                // chat message
                { MessageEventNames.MessageSend, new ChatMessageSerializer() },
                // tip add
                { MessageEventNames.TipAdd, new TipAddMessageSerializer() },
                // entity updates
                { MessageEventNames.SubscriberProfileUpdate, new UserUpdateMessageSerializer() },
                { MessageEventNames.GroupCreate, new GroupEditMessageSerializer<GroupCreateMessage>() },
                { MessageEventNames.GroupProfileUpdate, new GroupEditMessageSerializer<GroupUpdateMessage>() },
                { MessageEventNames.MessageUpdate, new ChatUpdateMessageSerializer() }
            };
        }

        /// <summary>Creates default message serializer map.</summary>
        /// <param name="additionalMappings">Additional mappings. Can overwrite default mappings.</param>
        /// <param name="fallbackSerializer">Serializer to use as fallback. If null, 
        /// <see cref="DefaultMessageSerializer{T}"/> for <see cref="IWolfMessage"/> will be used.</param>
        public DefaultMessageSerializerMap(IEnumerable<KeyValuePair<string, IMessageSerializer>> additionalMappings, 
            IMessageSerializer fallbackSerializer = null) : this(fallbackSerializer)
        {
            foreach (var pair in additionalMappings)
                this.MapSerializer(pair.Key, pair.Value);
        }

        /// <inheritdoc/>
        public IMessageSerializer FindMappedSerializer(string key)
        {
            this._map.TryGetValue(key, out IMessageSerializer result);
            return result;
        }

        /// <inheritdoc/>
        public void MapSerializer(string key, IMessageSerializer serializer)
            => this._map[key] = serializer;
    }
}
