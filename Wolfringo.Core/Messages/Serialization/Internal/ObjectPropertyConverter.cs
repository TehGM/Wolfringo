using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter that serializes and deserializes just one of the json object's properties.</summary>
    public class ObjectPropertyConverter : JsonConverter
    {
        private readonly string _propName;

        /// <inheritdoc/>
        /// <param name="propertyName">Name of the property to use.</param>
        public ObjectPropertyConverter(string propertyName)
        {
            this._propName = propertyName;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject obj = new JObject(new JProperty(_propName, JToken.FromObject(value, serializer)));
            obj.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            token = token[_propName];
            return token.ToObject(objectType);
        }
    }
}
