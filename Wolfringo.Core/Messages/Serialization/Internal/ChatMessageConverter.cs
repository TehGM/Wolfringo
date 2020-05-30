using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    public class ChatMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (typeof(IChatMessage).IsAssignableFrom(objectType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value, serializer);
            t.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jsonObject = JToken.Load(reader);
            if (jsonObject == null)
                return default;
            Type msgType = ChatMessageSerializer.GetMessageType(jsonObject);
            if (msgType == null)
                return default;

            object result = jsonObject.ToObject(msgType, serializer);
            jsonObject.FlattenCommonProperties(ref result);
            return result;
        }
    }
}
