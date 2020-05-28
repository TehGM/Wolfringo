using System;
using System.Collections.Generic;

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
                { MessageCommands.SecurityLogin, new DefaultMessageSerializer<LoginMessage>() },
                { MessageCommands.MessagePrivateSubscribe, new DefaultMessageSerializer<SubscribeToPmMessage>() },
                { MessageCommands.MessageGroupSubscribe, new DefaultMessageSerializer<SubscribeToGroupMessage>() },
                { MessageCommands.MessageSend, new ChatMessageSerializer() },
                { MessageCommands.NotificationList, new DefaultMessageSerializer<ListNotificationsMessage>() },
                { MessageCommands.SubscriberProfile, new DefaultMessageSerializer<UserProfileMessage>() },
                { MessageCommands.SubscriberContactList, new DefaultMessageSerializer<ContactListMessage>() },
                { MessageCommands.PresenceUpdate, new DefaultMessageSerializer<PresenceUpdateMessage>() },
                { MessageCommands.SubscriberSettingsUpdate, new DefaultMessageSerializer<OnlineStateUpdateMessage>() },
                { MessageCommands.SecurityLogout, new DefaultMessageSerializer<LogoutMessage>() }
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
