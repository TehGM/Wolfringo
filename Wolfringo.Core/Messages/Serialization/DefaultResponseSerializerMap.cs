using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class DefaultResponseSerializerMap : ISerializerMap<Type, IResponseSerializer>
    {
        public IResponseSerializer FallbackSerializer { get; set; }

        private IDictionary<Type, IResponseSerializer> _map;

        public DefaultResponseSerializerMap(IResponseSerializer fallbackSerializer = null)
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
                { typeof(GroupProfileResponse), defaultSerializer },
                { typeof(GroupMembersListResponse), defaultSerializer },
                { typeof(GroupListResponse), defaultSerializer },
                { typeof(CharmListResponse), defaultSerializer },
                { typeof(CharmStatisticsResponse), defaultSerializer },
                // chat history
                { typeof(ChatHistoryResponse), chatHistorySerializer },
                { typeof(RecentConversationsResponse), chatHistorySerializer },
                // entity updates
                { typeof(UserUpdateResponse), new UserUpdateResponseSerializer() }
            };
        }

        public DefaultResponseSerializerMap(IEnumerable<KeyValuePair<Type, IResponseSerializer>> additionalMappings, 
            IResponseSerializer fallbackSerializer = null) : this(fallbackSerializer)
        {
            foreach (var pair in additionalMappings)
                this.MapSerializer(pair.Key, pair.Value);
        }

        public IResponseSerializer FindMappedSerializer(Type key)
        {
            this._map.TryGetValue(key, out IResponseSerializer result);
            return result;
        }

        public void MapSerializer(Type key, IResponseSerializer serializer)
            => this._map[key] = serializer;
    }
}
