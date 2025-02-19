using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Options for default response serializer provider.</summary>
    /// <seealso cref="ResponseSerializerProvider"/>
    public class ResponseSerializerProviderOptions
    {
        /// <summary>Deserializer of chat embeds that will be used by serializers.</summary>
        public static IChatEmbedDeserializer ChatEmbedDeserializer { get; } = Serialization.ChatEmbedDeserializer.Instance;

        /// <summary>Default serializer.</summary>
        /// <remarks>This serializer is used for multiple mappings in default <see cref="Serializers"/>.</remarks>
        protected static IResponseSerializer DefaultSerializer { get; } = new DefaultResponseSerializer();
        /// <summary>Default chat history serializer.</summary>
        /// <remarks>This serializer is used for multiple mappings in default <see cref="Serializers"/>.</remarks>
        protected static IResponseSerializer DefaultHistorySerializer { get; }

        /// <summary>Fallback serializer that can be used if key has no mapped serializer.</summary>
        /// <remarks><para>Note that this serializer cannot be used for deserialization, and will be used only for serialization.</para>
        /// <para>Defaults to <see cref="DefaultResponseSerializer"/>.</para></remarks>
        public IResponseSerializer FallbackSerializer { get; set; } = DefaultSerializer;

        /// <summary>Map for response type and assigned response serializer.</summary>
        // TODO: 3.0: refactor to work on System.Type so it can be used with IServiceProvider instead
        public IDictionary<Type, IResponseSerializer> Serializers { get; set; }

        static ResponseSerializerProviderOptions()
        {
            DefaultHistorySerializer = new ChatHistoryResponseSerializer(ChatEmbedDeserializer);
        }

        /// <summary>Initializes a new instance of options using default values.</summary>
        public ResponseSerializerProviderOptions()
        {
            this.Serializers = new Dictionary<Type, IResponseSerializer>()
            {
                // default
                { typeof(WolfResponse), DefaultSerializer },
                { typeof(ChatResponse), DefaultSerializer },
                { typeof(LoginResponse), DefaultSerializer },
                { typeof(NotificationsListResponse), DefaultSerializer },
                { typeof(UserProfileResponse), DefaultSerializer },
                { typeof(ContactListResponse), DefaultSerializer },
                { typeof(OnlineStateUpdateResponse), DefaultSerializer },
                { typeof(GroupMembersListResponse), DefaultSerializer },
                { typeof(GroupListResponse), DefaultSerializer },
                { typeof(CharmListResponse), DefaultSerializer },
                { typeof(CharmStatisticsResponse), DefaultSerializer },
                { typeof(BlockListResponse), DefaultSerializer },
                { typeof(GroupAudioUpdateResponse), DefaultSerializer },
                { typeof(AchievementListResponse), DefaultSerializer },
                { typeof(EntityAchievementListResponse), DefaultSerializer },
                { typeof(UserCharmsListResponse), DefaultSerializer },
                { typeof(EntitiesSubscribeResponse), DefaultSerializer },
                { typeof(TipSummaryResponse), DefaultSerializer },
                { typeof(UrlMetadataResponse), DefaultSerializer },
                // group stats
                { typeof(GroupStatisticsResponse), new GroupStatisticsResponseSerializer() },
                // group profile
                { typeof(GroupProfileResponse), new GroupProfileResponseSerializer() },
                // chat history
                { typeof(ChatHistoryResponse), DefaultHistorySerializer },
                { typeof(RecentConversationsResponse), DefaultHistorySerializer },
                // entity updates
                { typeof(UserUpdateResponse), new UserUpdateResponseSerializer() },
                { typeof(GroupEditResponse), new GroupEditResponseSerializer() },
                { typeof(ChatUpdateResponse), new ChatUpdateResponseSerializer() },
                // tips
                { typeof(TipDetailsResponse), new TipDetailsResponseSerializer() },
            };

        }
    }
}
