﻿using System;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Collection of message event names used by Wolf protocol.</summary>
    public static class MessageEventNames
    {
        /// <summary>WOLF Protocol Event name for <see cref="WelcomeEvent"/>.</summary>
        public const string Welcome = "welcome";
        /// <summary>WOLF Protocol Event name for <see cref="SubscribeToPmMessage"/>.</summary>
        public const string MessagePrivateSubscribe = "message private subscribe";
        /// <summary>WOLF Protocol Event name for <see cref="SubscribeToGroupMessage"/>.</summary>
        public const string MessageGroupSubscribe = "message group subscribe";
        /// <summary>WOLF Protocol Event name for <see cref="LoginMessage"/>.</summary>
        public const string SecurityLogin = "security login";
        /// <summary>WOLF Protocol Event name for <see cref="LogoutMessage"/>.</summary>
        public const string SecurityLogout = "security logout";
        /// <summary>WOLF Protocol Event name for <see cref="ChatMessage"/>.</summary>
        public const string MessageSend = "message send";
        /// <summary>WOLF Protocol Event name for <see cref="NotificationsListMessage"/>.</summary>
        public const string NotificationList = "notification list";
        /// <summary>WOLF Protocol Event name for <see cref="UserProfileMessage"/>.</summary>
        public const string SubscriberProfile = "subscriber profile";
        /// <summary>WOLF Protocol Event name for <see cref="GroupProfileMessage"/>.</summary>
        public const string GroupProfile = "group profile";
        /// <summary>WOLF Protocol Event name for <see cref="ContactListMessage"/>.</summary>
        public const string SubscriberContactList = "subscriber contact list";
        /// <summary>WOLF Protocol Event name for <see cref="PresenceUpdateEvent"/>.</summary>
        public const string PresenceUpdate = "presence update";
        /// <summary>WOLF Protocol Event name for <see cref="OnlineStateUpdateMessage"/>.</summary>
        public const string SubscriberSettingsUpdate = "subscriber settings update";
        /// <summary>WOLF Protocol Event name for <see cref="GroupAudioCountUpdateEvent"/>.</summary>
        public const string GroupAudioCountUpdate = "group audio count update";
        /// <summary>WOLF Protocol Event name for <see cref="GroupUpdateEvent"/>.</summary>
        public const string GroupUpdate = "group update";
        /// <summary>WOLF Protocol Event name for <see cref="GroupMembersListMessage"/>.</summary>
        public const string GroupMemberList = "group member list";
        /// <summary>WOLF Protocol Event name for <see cref="GroupMemberPrivilegedListMessage"/>.</summary>
        public const string GroupMemberPrivilegedList = "group member privileged list";
        /// <summary>WOLF Protocol Event name for <see cref="GroupMemberRegularListMessage"/>.</summary>
        public const string GroupMemberRegularList = "group member regular list";
        /// <summary>WOLF Protocol Event name for <see cref="GroupJoinMessage"/>.</summary>
        public const string GroupMemberAdd = "group member add";
        /// <summary>WOLF Protocol Event name for <see cref="GroupLeaveMessage"/>.</summary>
        public const string GroupMemberDelete = "group member delete";
        /// <summary>WOLF Protocol Event name for <see cref="GroupMemberUpdateEvent"/>.</summary>
        [Obsolete("WOLF protocol seems to no longer send this event. Use GroupMemberPrivilegedUpdate and GroupMemberPrivilegedDelete instead.")]
        public const string GroupMemberUpdate = "group member update";
        /// <summary>WOLF Protocol Event name for <see cref="GroupMemberPrivilegedUpdateEvent"/>.</summary>
        public const string GroupMemberPrivilegedUpdate = "group member privileged update";
        /// <summary>WOLF Protocol Event name for <see cref="GroupMemberPrivilegedDeleteEvent"/>.</summary>
        public const string GroupMemberPrivilegedDelete = "group member privileged delete";
        /// <summary>WOLF Protocol Event name for <see cref="GroupAdminMessage"/>.</summary>
        public const string GroupAdmin = "group admin";
        /// <summary>WOLF Protocol Event name for <see cref="PrivateChatHistoryMessage"/>.</summary>
        public const string MessagePrivateHistoryList = "message private history list";
        /// <summary>WOLF Protocol Event name for <see cref="GroupChatHistoryMessage"/>.</summary>
        public const string MessageGroupHistoryList = "message group history list";
        /// <summary>WOLF Protocol Event name for <see cref="RecentConversationsMessage"/>.</summary>
        public const string MessageConversationList = "message conversation list";
        /// <summary>WOLF Protocol Event name for <see cref="GroupListMessage"/>.</summary>
        public const string SubscriberGroupList = "subscriber group list";
        /// <summary>WOLF Protocol Event name for <see cref="CharmListMessage"/>.</summary>
        public const string CharmList = "charm list";
        /// <summary>WOLF Protocol Event name for <see cref="CharmStatisticsMessage"/>.</summary>
        public const string CharmSubscriberStatistics = "charm subscriber statistics";
        /// <summary>WOLF Protocol Event name for <see cref="UserProfileMessage"/>.</summary>
        public const string SubscriberProfileUpdate = "subscriber profile update";
        /// <summary>WOLF Protocol Event name for <see cref="UserUpdateEvent"/>.</summary>
        public const string SubscriberUpdate = "subscriber update";
        /// <summary>WOLF Protocol Event name for <see cref="NotificationsClearMessage"/>.</summary>
        public const string NotificationListClear = "notification list clear";
        /// <summary>WOLF Protocol Event name for <see cref="UnsubscribeFromGroupMessage"/>.</summary>
        public const string MessageGroupUnsubscribe = "message group unsubscribe";
        /// <summary>WOLF Protocol Event name for <see cref="UnsubscribeFromPrivateMessage"/>.</summary>
        public const string MessagePrivateUnsubscribe = "message private unsubscribe";
        /// <summary>WOLF Protocol Event name for <see cref="ContactAddMessage"/>.</summary>
        public const string SubscriberContactAdd = "subscriber contact add";
        /// <summary>WOLF Protocol Event name for <see cref="ContactDeleteMessage"/>.</summary>
        public const string SubscriberContactDelete = "subscriber contact delete";
        /// <summary>WOLF Protocol Event name for <see cref="BlockListMessage"/>.</summary>
        public const string SubscriberBlockList = "subscriber block list";
        /// <summary>WOLF Protocol Event name for <see cref="BlockAddMessage"/>.</summary>
        public const string SubscriberBlockAdd = "subscriber block add";
        /// <summary>WOLF Protocol Event name for <see cref="BlockDeleteMessage"/>.</summary>
        public const string SubscriberBlockDelete = "subscriber block delete";
        /// <summary>WOLF Protocol Event name for <see cref="GroupCreateMessage"/>.</summary>
        public const string GroupCreate = "group create";
        /// <summary>WOLF Protocol Event name for <see cref="GroupUpdateMessage"/>.</summary>
        public const string GroupProfileUpdate = "group profile update";
        /// <summary>WOLF Protocol Event name for <see cref="GroupAudioUpdateMessage"/>.</summary>
        public const string GroupAudioUpdate = "group audio update";
        /// <summary>WOLF Protocol Event name for <see cref="AchievementMessage"/>.</summary>
        public const string Achievement = "achievement";
        /// <summary>WOLF Protocol Event name for <see cref="AchievementListMessage"/>.</summary>
        public const string AchievementList = "achievement list";
        /// <summary>WOLF Protocol Event name for <see cref="UserAchievementListMessage"/>.</summary>
        public const string AchievementSubscriberList = "achievement subscriber list";
        /// <summary>WOLF Protocol Event name for <see cref="GroupAchievementListMessage"/>.</summary>
        public const string AchievementGroupList = "achievement group list";
        /// <summary>WOLF Protocol Event name for <see cref="GroupStatisticsMessage"/>.</summary>
        public const string GroupStats = "group stats";
        /// <summary>WOLF Protocol Event name for <see cref="UserExpiredCharmsListMessage"/>.</summary>
        public const string CharmSubscriberExpiredList = "charm subscriber expired list";
        /// <summary>WOLF Protocol Event name for <see cref="UserActiveCharmsListMessage"/>.</summary>
        public const string CharmSubscriberActiveList = "charm subscriber active list";
        /// <summary>WOLF Protocol Event name for <see cref="UserCharmsSelectMessage"/>.</summary>
        public const string CharmSubscriberSetSelected = "charm subscriber set selected";
        /// <summary>WOLF Protocol Event name for <see cref="ChatUpdateMessage"/>.</summary>
        public const string MessageUpdate = "message update";
        /// <summary>WOLF Protocol Event name for <see cref="SubscribeToGroupTipsMessage"/>.</summary>
        public const string TipGroupSubscribe = "tip group subscribe";
        /// <summary>WOLF Protocol Event name for <see cref="TipSummaryMessage"/>.</summary>
        public const string TipSummary = "tip summary";
        /// <summary>WOLF Protocol Event name for <see cref="TipDetailsMessage"/>.</summary>
        public const string TipDetail = "tip detail";
        /// <summary>WOLF Protocol Event name for <see cref="TipAddMessage"/>.</summary>
        public const string TipAdd = "tip add";
        /// <summary>WOLF Protocol Event name for <see cref="UrlMetadataMessage"/>.</summary>
        public const string MetadataUrl = "metadata url";
    }
}
