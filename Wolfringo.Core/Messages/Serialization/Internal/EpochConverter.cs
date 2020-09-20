using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for converting unix epoch to DateTime.</summary>
    public class EpochConverter : DateTimeConverterBase
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double seconds = ((DateTime)value - WolfTimestamp.Epoch).TotalSeconds;
            writer.WriteValue((long)seconds);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return WolfTimestamp.Epoch.AddSeconds((long)reader.Value);
        }
    }
}
