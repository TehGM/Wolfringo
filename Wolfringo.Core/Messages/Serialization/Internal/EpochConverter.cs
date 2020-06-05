using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for converting unix epoch to DateTime.</summary>
    public class EpochConverter : DateTimeConverterBase
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double seconds = ((DateTime)value - Epoch).TotalSeconds;
            writer.WriteValue((long)seconds);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return Epoch.AddSeconds((long)reader.Value);
        }
    }
}
