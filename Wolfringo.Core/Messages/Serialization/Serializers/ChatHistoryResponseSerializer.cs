using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for chat history responses.</summary>
    /// <remarks>This special serializer parses binary messages for all nested chat messages in chat history array.</remarks>
    public class ChatHistoryResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _chatHistoryResponseType = typeof(ChatHistoryResponse);

        private readonly IChatEmbedDeserializer _chatEmbedDeserializer;

        /// <summary>Initializes a new serializer for chat history responses.</summary>
        /// <param name="chatEmbedDeserializer">Deserializer of chat embeds to use.</param>
        public ChatHistoryResponseSerializer(IChatEmbedDeserializer chatEmbedDeserializer)
        {
            this._chatEmbedDeserializer = chatEmbedDeserializer;
        }

        /// <summary>Initializes a new serializer for chat history responses.</summary>
        /// <remarks>This uses default <see cref="ChatEmbedDeserializer"/> for deserializing embeds.</remarks>
        public ChatHistoryResponseSerializer()
            : this(ChatEmbedDeserializer.Instance) { }

        /// <inheritdoc/>
        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            JArray responseBody = GetResponseJson(responseData)["body"] as JArray;
            if (responseBody == null)
                throw new ArgumentException("Chat history response requires to have a body property that is a JSON array", nameof(responseData));

            Dictionary<JToken, IEnumerable<IChatEmbed>> extractedEmbeds = new Dictionary<JToken, IEnumerable<IChatEmbed>>(responseBody.Count);
            foreach (JToken responseChatMessage in responseBody)
            {
                if (!(responseChatMessage is JObject chatMessageObject))
                    continue;

                IEnumerable<IChatEmbed> embeds = this._chatEmbedDeserializer.DeserializeEmbeds(chatMessageObject).ToArray();
                chatMessageObject.Remove("embeds");
                extractedEmbeds.Add(chatMessageObject, embeds);
            }

            ChatHistoryResponse result = (ChatHistoryResponse)base.Deserialize(responseType, responseData);
            foreach (JToken responseChatMessage in responseBody)
            {
                WolfTimestamp msgTimestamp = responseChatMessage["timestamp"].ToObject<WolfTimestamp>(SerializationHelper.DefaultSerializer);
                IChatMessage msg = result.Messages.First(m => m.Timestamp == msgTimestamp);
                JToken numProp = responseChatMessage["data"]?["num"];
                if (numProp != null)
                {
                    int binaryIndex = numProp.ToObject<int>(SerializationHelper.DefaultSerializer);
                    SerializationHelper.PopulateMessageRawData(ref msg, responseData.BinaryMessages.ElementAt(binaryIndex));
                }
                if (msg is ChatMessage chatMsg && extractedEmbeds.TryGetValue(responseChatMessage, out IEnumerable<IChatEmbed> embeds))
                {
                    this._chatEmbedDeserializer.PopulateMessageEmbeds(ref chatMsg, embeds);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_chatHistoryResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_chatHistoryResponseType.FullName}", nameof(responseType));
        }
    }
}
