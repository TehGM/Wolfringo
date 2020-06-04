using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    public class ValueOrPropertyConverter : JsonConverter
    {
        private readonly string _propPath;

        public ValueOrPropertyConverter(string propertyPath)
        {
            this._propPath = propertyPath;
        }

        public override bool CanConvert(Type objectType)
        {
            return (Type.GetTypeCode(objectType) != TypeCode.Object);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
                token = token.SelectToken(_propPath);
            return token?.ToObject(objectType, serializer);
        }
    }
}
