using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Types;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class DefaultMessageSerializerMap : ISerializerMap<string, IMessageSerializer>
    {
        public IMessageSerializer FallbackSerializer { get; set; }

        private IDictionary<string, IMessageSerializer> _map;

        public DefaultMessageSerializerMap(IMessageSerializer fallbackSerializer = null)
        {
            this.FallbackSerializer = fallbackSerializer ?? new DefaultMessageSerializer<IWolfMessage>();
            this._map = new Dictionary<string, IMessageSerializer>(StringComparer.OrdinalIgnoreCase)
            {
                { MessageCommands.Welcome, new DefaultMessageSerializer<WelcomeMessage>() },
                { MessageCommands.Login, new DefaultMessageSerializer<LoginMessage>() },
                { MessageCommands.SubscribeToPm, new DefaultMessageSerializer<SubscribeToPmMessage>() },
                { MessageCommands.SubscribeToGroup, new DefaultMessageSerializer<SubscribeToGroupMessage>() },
                { MessageCommands.Chat, new ChatMessageSerializer() },
                { MessageCommands.ListNotifications, new DefaultMessageSerializer<ListNotificationsMessage>() },
                { MessageCommands.SubscriberProfile, new DefaultMessageSerializer<SubscriberProfileMessage>() },
                { MessageCommands.ContactList, new DefaultMessageSerializer<ContactListMessage>() }
            };
        }

        public DefaultMessageSerializerMap(IEnumerable<KeyValuePair<string, IMessageSerializer>> additionalMappings, 
            IMessageSerializer fallbackSerializer = null) : this(fallbackSerializer)
        {
            foreach (var pair in additionalMappings)
                this.MapSerializer(pair.Key, pair.Value);
        }

        public IMessageSerializer FindMappedSerializer(string key)
        {
            this._map.TryGetValue(key, out IMessageSerializer result);
            return result;
        }

        public void MapSerializer(string key, IMessageSerializer serializer)
            => this._map[key] = serializer;
    }
}
