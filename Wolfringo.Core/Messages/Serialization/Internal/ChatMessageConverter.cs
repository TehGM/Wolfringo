using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for chat messages.</summary>
    /// <remarks>This custom converter designed for <see cref="IChatMessage"/> uses <see cref="ChatMessage"/> to choose correct message type.</remarks>
    public class ChatMessageConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return (typeof(IChatMessage).IsAssignableFrom(objectType));
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value, serializer);
            t.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jsonObject = JToken.Load(reader);
            if (jsonObject == null)
                return default;
            Type msgType = ChatMessageSerializer.GetMessageType(jsonObject);
            if (msgType == null)
                return default;

            object result = jsonObject.ToObject(msgType, serializer);
            jsonObject.FlattenCommonProperties(result);
            return result;
        }
    }
}
