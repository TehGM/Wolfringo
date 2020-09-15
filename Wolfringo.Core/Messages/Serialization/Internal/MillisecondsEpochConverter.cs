using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    // https://stackoverflow.com/questions/19971494/how-to-deserialize-a-unix-timestamp-%CE%BCs-to-a-datetime-from-json/56795442

    /// <summary>Json converter for converting unix epoch in millisecond format to DateTime.</summary>
    public class MillisecondsEpochConverter : EpochConverter
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double ticks = ((DateTime)value - Epoch).Ticks;
            writer.WriteValue((long)ticks / 10);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return Epoch.AddTicks((long)reader.Value * 10);
        }
    }
}
