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
            this.FallbackSerializer = fallbackSerializer ?? defaultSerializer;
            this._map = new Dictionary<Type, IResponseSerializer>()
            {
                { typeof(WolfResponse), defaultSerializer },
                { typeof(ChatResponse), defaultSerializer },
                { typeof(LoginResponse), defaultSerializer },
                { typeof(ListNotificationsResponse), defaultSerializer }
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
