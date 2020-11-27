using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Default response type to response serializer map.</summary>
    /// <remarks><para>This class contains all Wolfringo library default response mappings, and will be used by default clients
    /// if no other map is provided.</para>
    /// <para>This class can be easily extended without inheritance. To provide custom mappings, call <see cref="MapSerializer(Type, IResponseSerializer)"/>, 
    /// or pass custom mappings dictionary via the constructor. However, usage with .NET Core Host might require inheriting 
    /// and registering as a service in the service container - in this case, simply call <see cref="MapSerializer(Type, IResponseSerializer)"/>
    /// in child class constructor for each custom mapping that needs to be made.</para></remarks>
    public class ResponseSerializerProvider : ISerializerProvider<Type, IResponseSerializer>
    {
        /// <inheritdoc/>
        public IResponseSerializer FallbackSerializer { get; set; }

        private IDictionary<Type, IResponseSerializer> _map;

        /// <summary>Creates default response serializer map.</summary>
        /// <param name="fallbackSerializer">Serializer to use as fallback. If null, <see cref="DefaultResponseSerializer"/> will be used.</param>
        public ResponseSerializerProvider(IResponseSerializer fallbackSerializer = null)
        {
            DefaultResponseSerializer defaultSerializer = new DefaultResponseSerializer();
            ChatHistoryResponseSerializer chatHistorySerializer = new ChatHistoryResponseSerializer();
            this.FallbackSerializer = fallbackSerializer ?? defaultSerializer;
            this._map = new Dictionary<Type, IResponseSerializer>()
            {
                // default
                { typeof(WolfResponse), defaultSerializer },
                { typeof(ChatResponse), defaultSerializer },
                { typeof(LoginResponse), defaultSerializer },
                { typeof(NotificationsListResponse), defaultSerializer },
                { typeof(UserProfileResponse), defaultSerializer },
                { typeof(ContactListResponse), defaultSerializer },
                { typeof(OnlineStateUpdateResponse), defaultSerializer },
                { typeof(GroupMembersListResponse), defaultSerializer },
                { typeof(GroupListResponse), defaultSerializer },
                { typeof(CharmListResponse), defaultSerializer },
                { typeof(CharmStatisticsResponse), defaultSerializer },
                { typeof(BlockListResponse), defaultSerializer },
                { typeof(GroupAudioUpdateResponse), defaultSerializer },
                { typeof(AchievementListResponse), defaultSerializer },
                { typeof(UserAchievementListResponse), defaultSerializer },
                { typeof(UserCharmsListResponse), defaultSerializer },
                { typeof(EntitiesSubscribeResponse), defaultSerializer },
                { typeof(TipSummaryResponse), defaultSerializer },
                // group stats
                { typeof(GroupStatisticsResponse), new GroupStatisticsResponseSerializer() },
                // group profile
                { typeof(GroupProfileResponse), new GroupProfileResponseSerializer() },
                // chat history
                { typeof(ChatHistoryResponse), chatHistorySerializer },
                { typeof(RecentConversationsResponse), chatHistorySerializer },
                // entity updates
                { typeof(UserUpdateResponse), new UserUpdateResponseSerializer() },
                { typeof(GroupEditResponse), new GroupEditResponseSerializer() },
                { typeof(ChatUpdateResponse), new ChatUpdateResponseSerializer() },
                // tips
                { typeof(TipDetailsResponse), new TipDetailsResponseSerializer() },
            };
        }

        /// <summary>Creates default response serializer map.</summary>
        /// <param name="additionalMappings">Additional mappings. Can overwrite default mappings.</param>
        /// <param name="fallbackSerializer">Serializer to use as fallback. If null, <see cref="DefaultResponseSerializer"/> will be used.</param>
        public ResponseSerializerProvider(IEnumerable<KeyValuePair<Type, IResponseSerializer>> additionalMappings, 
            IResponseSerializer fallbackSerializer = null) : this(fallbackSerializer)
        {
            foreach (var pair in additionalMappings)
                this.MapSerializer(pair.Key, pair.Value);
        }

        /// <inheritdoc/>
        public IResponseSerializer FindMappedSerializer(Type key)
        {
            this._map.TryGetValue(key, out IResponseSerializer result);
            return result;
        }

        /// <inheritdoc/>
        public void MapSerializer(Type key, IResponseSerializer serializer)
            => this._map[key] = serializer;
    }
}
