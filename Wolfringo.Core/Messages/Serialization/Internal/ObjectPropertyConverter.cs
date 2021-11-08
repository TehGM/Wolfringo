using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter that serializes and deserializes just one of the json object's properties.</summary>
    public class ObjectPropertyConverter : JsonConverter
    {
        private readonly string _propPath;

        /// <inheritdoc/>
        /// <param name="propertyPath">Path of the property to use.</param>
        public ObjectPropertyConverter(string propertyPath)
        {
            this._propPath = propertyPath;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject obj = new JObject();
            obj.AddAtPath(this._propPath, value != null ? JToken.FromObject(value, serializer) : null);
            obj.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            token = token.SelectToken(this._propPath);
            return token.ToObject(objectType, serializer);
        }
    }
}
