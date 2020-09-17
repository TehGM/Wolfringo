using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for converting unix epoch in millisecond format to WolfTimestamp or DateTime.</summary>
    public class WolfTimestampConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((long)((WolfTimestamp)value));
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            WolfTimestamp result = new WolfTimestamp((long)reader.Value);
            if (objectType.IsAssignableFrom(typeof(long)))
                return (long)result;
            if (objectType.IsAssignableFrom(typeof(DateTime)))
                return (DateTime)result;
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(WolfTimestamp).IsAssignableFrom(objectType) || typeof(long).IsAssignableFrom(objectType) || typeof(DateTime).IsAssignableFrom(objectType);
        }
    }
}
