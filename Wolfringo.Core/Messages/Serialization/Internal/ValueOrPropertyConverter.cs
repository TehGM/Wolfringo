using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for retrieving specified property if the token is an object.</summary>
    public class ValueOrPropertyConverter : JsonConverter
    {
        private readonly string _propPath;

        /// <inheritdoc/>
        /// <param name="propertyPath">Path to the property to retrieve if token is an object.</param>
        public ValueOrPropertyConverter(string propertyPath)
        {
            this._propPath = propertyPath;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return (Type.GetTypeCode(objectType) != TypeCode.Object);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
                token = token.SelectToken(_propPath);
            return token?.ToObject(objectType, serializer);
        }
    }
}
