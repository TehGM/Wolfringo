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
                { MessageCommands.Welcome, new DefaultMessageSerializer<WelcomeEvent>() },
                { MessageCommands.SecurityLogin, new DefaultMessageSerializer<LoginMessage>() },
                { MessageCommands.MessagePrivateSubscribe, new DefaultMessageSerializer<SubscribeToPmMessage>() },
                { MessageCommands.MessageGroupSubscribe, new DefaultMessageSerializer<SubscribeToGroupMessage>() },
                { MessageCommands.NotificationList, new DefaultMessageSerializer<NotificationsListMessage>() },
                { MessageCommands.SubscriberProfile, new DefaultMessageSerializer<UserProfileMessage>() },
                { MessageCommands.SubscriberContactList, new DefaultMessageSerializer<ContactListMessage>() },
                { MessageCommands.PresenceUpdate, new DefaultMessageSerializer<PresenceUpdateEvent>() },
                { MessageCommands.SubscriberSettingsUpdate, new DefaultMessageSerializer<OnlineStateUpdateMessage>() },
                { MessageCommands.SecurityLogout, new DefaultMessageSerializer<LogoutMessage>() },
                { MessageCommands.GroupProfile, new DefaultMessageSerializer<GroupProfileMessage>() },
                { MessageCommands.GroupAudioCountUpdate, new DefaultMessageSerializer<GroupAudioCountUpdateEvent>() },
                { MessageCommands.GroupUpdate, new DefaultMessageSerializer<GroupUpdateEvent>() },
                { MessageCommands.GroupMemberList, new DefaultMessageSerializer<GroupMembersListMessage>() },
                { MessageCommands.MessageGroupHistoryList, new DefaultMessageSerializer<GroupChatHistoryMessage>() },
                { MessageCommands.MessagePrivateHistoryList, new DefaultMessageSerializer<PrivateChatHistoryMessage>() },
                { MessageCommands.MessageConversationList, new DefaultMessageSerializer<RecentConversationsMessage>() },
                { MessageCommands.SubscriberGroupList, new DefaultMessageSerializer<GroupListMessage>() },
                { MessageCommands.CharmList, new DefaultMessageSerializer<CharmListMessage>() },
                { MessageCommands.CharmSubscriberStatistics, new DefaultMessageSerializer<CharmStatisticsMessage>() },
                { MessageCommands.GroupMemberUpdate, new DefaultMessageSerializer<GroupMemberUpdateEvent>() },
                { MessageCommands.GroupAdmin, new DefaultMessageSerializer<GroupAdminMessage>() },
                { MessageCommands.SubscriberUpdate, new DefaultMessageSerializer<UserUpdateEvent>() },
                { MessageCommands.NotificationListClear, new DefaultMessageSerializer<NotificationsClearMessage>() },
                { MessageCommands.MessageGroupUnsubscribe, new DefaultMessageSerializer<UnsubscribeFromGroupMessage>() },
                { MessageCommands.MessagePrivateUnsubscribe, new DefaultMessageSerializer<UnsubscribeFromPrivateMessage>() },
                { MessageCommands.SubscriberBlockList, new DefaultMessageSerializer<BlockListMessage>() },
                { MessageCommands.SubscriberBlockAdd, new DefaultMessageSerializer<BlockAddMessage>() },
                { MessageCommands.SubscriberBlockDelete, new DefaultMessageSerializer<BlockDeleteMessage>() },
                { MessageCommands.GroupAudioUpdate, new DefaultMessageSerializer<GroupAudioUpdateMessage>() },
                { MessageCommands.AchievementList, new DefaultMessageSerializer<AchievementListMessage>() },
                { MessageCommands.AchievementSubscriberList, new DefaultMessageSerializer<UserAchievementListMessage>() },
                { MessageCommands.GroupStats, new DefaultMessageSerializer<GroupStatisticsMessage>() },
                { MessageCommands.CharmSubscriberExpiredList, new DefaultMessageSerializer<UserExpiredCharmsListMessage>() },
                { MessageCommands.CharmSubscriberActiveList, new DefaultMessageSerializer<UserActiveCharmsListMessage>() },
                { MessageCommands.CharmSubscriberSetSelected, new DefaultMessageSerializer<UserCharmsSelectMessage>() },
                // group join and leave
                { MessageCommands.GroupMemberAdd, new GroupJoinLeaveMessageSerializer<GroupJoinMessage>() },
                { MessageCommands.GroupMemberDelete, new GroupJoinLeaveMessageSerializer<GroupLeaveMessage>() },
                // contact add and delete
                { MessageCommands.SubscriberContactAdd, new ContactAddDeleteMessageSerializer<ContactAddMessage>() },
                { MessageCommands.SubscriberContactDelete, new ContactAddDeleteMessageSerializer<ContactDeleteMessage>() },
                // chat message
                { MessageCommands.MessageSend, new ChatMessageSerializer() },
                // entity updates
                { MessageCommands.SubscriberProfileUpdate, new UserUpdateMessageSerializer() },
                { MessageCommands.GroupCreate, new GroupEditMessageSerializer<GroupCreateMessage>() },
                { MessageCommands.GroupProfileUpdate, new GroupEditMessageSerializer<GroupUpdateMessage>() },
                { MessageCommands.MessageUpdate, new ChatUpdateMessageSerializer() }
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
