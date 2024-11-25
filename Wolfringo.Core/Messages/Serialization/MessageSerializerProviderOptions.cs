using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Options for default message serializer provider.</summary>
    /// <seealso cref="MessageSerializerProvider"/>
    public class MessageSerializerProviderOptions
    {
        /// <summary>Fallback serializer that can be used if key has no mapped serializer.</summary>
        /// <remarks><para>Note that this serializer cannot be used for deserialization, and will be used only for serialization.</para>
        /// <para>Defaults to <see cref="DefaultMessageSerializer{T}"/>, where T is <see cref="IWolfMessage"/>.</para></remarks>
        public IMessageSerializer FallbackSerializer { get; set; } = new DefaultMessageSerializer<IWolfMessage>();

        /// <summary>Map for event type and assigned message serializer.</summary>
#pragma warning disable CS0618 // Type or member is obsolete
        public IDictionary<string, IMessageSerializer> Serializers { get; set; } = new Dictionary<string, IMessageSerializer>(StringComparer.OrdinalIgnoreCase)
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
                { MessageEventNames.GroupMemberPrivilegedList, new DefaultMessageSerializer<GroupMemberPrivilegedListMessage>() },
                { MessageEventNames.GroupMemberRegularList, new DefaultMessageSerializer<GroupMemberRegularListMessage>() },
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
#pragma warning restore CS0618 // Type or member is obsolete
}
