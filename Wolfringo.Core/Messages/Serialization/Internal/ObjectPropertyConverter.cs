using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    public class ObjectPropertyConverter : JsonConverter
    {
        private readonly string _propName;

        public ObjectPropertyConverter(string propertyName)
        {
            this._propName = propertyName;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject obj = new JObject(new JProperty(_propName, JToken.FromObject(value, serializer)));
            obj.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            token = token[_propName];
            return token.ToObject(objectType);
        }
    }
}
