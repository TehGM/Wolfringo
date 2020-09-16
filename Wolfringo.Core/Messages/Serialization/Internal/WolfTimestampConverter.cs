using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    // https://stackoverflow.com/questions/19971494/how-to-deserialize-a-unix-timestamp-%CE%BCs-to-a-datetime-from-json/56795442

    /// <summary>Json converter for converting unix epoch in millisecond format to DateTime.</summary>
    public class WolfTimestampConverter : EpochConverter
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(SerializationHelper.DateTimeToWolfTimestamp((DateTime)value));
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return SerializationHelper.WolfTimestampToDateTime((long)reader.Value);
        }
    }
}
