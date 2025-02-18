using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Embeds;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Maps values present in 'type' property of chat embeds to concrete type for deserialization.</summary>
    public class ChatEmbedTypeMap
    {
        private readonly Dictionary<string, Type> _registeredEmbedTypes = new Dictionary<string, Type>()
        {
            ["linkPreview"] = typeof(LinkPreviewChatEmbed),
            ["imagePreview"] = typeof(ImagePreviewChatEmbed),
            ["groupPreview"] = typeof(GroupPreviewChatEmbed)
        };

        /// <summary>Attempts to retrieve the embed type.</summary>
        /// <param name="type">Value of 'type' property.</param>
        /// <param name="result">Resulting type if mapped.</param>
        /// <returns>Whether a type for given embed type was found.</returns>
        public bool TryGetChatEmbedType(string type, out Type result)
        {
            return this._registeredEmbedTypes.TryGetValue(type, out result);
        }

        /// <summary>Maps chat embed type to an implementation type.</summary>
        /// <typeparam name="T">Implementation type of the chat embed.</typeparam>
        /// <param name="type">Type of the chat embed.</param>
        public void MapChatEmbedType<T>(string type) where T : IChatEmbed
        {
            this._registeredEmbedTypes[type] = typeof(T);
        }
    }
}
