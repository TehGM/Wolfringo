using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Embeds;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Maps values present in 'type' property of chat embeds and provides means to deserialize them into a <see cref="ChatMessage"/>.</summary>
    public interface IChatEmbedDeserializer
    {
        /// <summary>Attempts to retrieve the embed type.</summary>
        /// <param name="type">Value of 'type' property.</param>
        /// <param name="result">Resulting type if mapped.</param>
        /// <returns>Whether a type for given embed type was found.</returns>
        bool TryGetChatEmbedType(string type, out Type result);
        /// <summary>Maps chat embed type to an implementation type.</summary>
        /// <typeparam name="T">Implementation type of the chat embed.</typeparam>
        /// <param name="type">Type of the chat embed.</param>
        void MapChatEmbedType<T>(string type) where T : IChatEmbed;
        /// <summary>deserializes all embeds from message body.</summary>
        /// <remarks>Embeds are deserialized if body has 'embeds' array. Otherwise an empty enumerable is returned.</remarks>
        /// <param name="messageBody">Body of the message.</param>
        /// <returns>Enumerable of chat embeds.</returns>
        IEnumerable<IChatEmbed> DeserializeEmbeds(JObject messageBody);
        /// <summary>Populates chat message's embeds.</summary>
        /// <param name="message">Chat message.</param>
        /// <param name="embeds">Deserialized embeds.</param>
        void PopulateMessageEmbeds(ref ChatMessage message, IEnumerable<IChatEmbed> embeds);
    }

    /// <inheritdoc/>
    public class ChatEmbedDeserializer : IChatEmbedDeserializer
    {
        internal static ChatEmbedDeserializer Instance { get; } = new ChatEmbedDeserializer();

        private readonly Dictionary<string, Type> _registeredEmbedTypes = new Dictionary<string, Type>()
        {
            ["linkPreview"] = typeof(LinkPreviewChatEmbed),
            ["imagePreview"] = typeof(ImagePreviewChatEmbed),
            ["groupPreview"] = typeof(GroupPreviewChatEmbed)
        };

        /// <inheritdoc/>
        public bool TryGetChatEmbedType(string type, out Type result)
        {
            return this._registeredEmbedTypes.TryGetValue(type, out result);
        }

        /// <inheritdoc/>
        public void MapChatEmbedType<T>(string type) where T : IChatEmbed
        {
            this._registeredEmbedTypes[type] = typeof(T);
        }

        /// <inheritdoc/>
        public IEnumerable<IChatEmbed> DeserializeEmbeds(JObject messageBody)
        {
            if (messageBody == null || !messageBody.ContainsKey("embeds") || !(messageBody["embeds"] is JArray embeds))
                yield break;

            foreach (JToken embed in embeds)
            {
                if (!(embed is JObject embedObject) || !embedObject.ContainsKey("type"))
                    continue;

                string embedType = embedObject["type"].ToObject<string>();
                if (string.IsNullOrWhiteSpace(embedType))
                    continue;

                if (this.TryGetChatEmbedType(embedType, out Type type))
                {
                    yield return (IChatEmbed)embedObject.ToObject(type, SerializationHelper.DefaultSerializer);
                }
            }
        }

        /// <inheritdoc/>
        public void PopulateMessageEmbeds(ref ChatMessage message, IEnumerable<IChatEmbed> embeds)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (embeds?.Any() != true)
                return;

            if (message.Embeds == null || !(message.Embeds is ICollection<IChatEmbed> embedCollection) || embedCollection.IsReadOnly)
                throw new InvalidOperationException($"Cannot populate embeds in {message.GetType().Name} as the collection is read only or null");
            embedCollection.Clear();

            // if it's a list, we can do it in a more performant way
            if (message.Embeds is List<IChatEmbed> embedList)
                embedList.AddRange(embeds);
            // otherwise do it one by one
            else
            {
                foreach (IChatEmbed e in embeds)
                    embedCollection.Add(e);
            }
        }
    }
}
