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
            double ms = ((DateTime)value - Epoch).TotalMilliseconds;
            writer.WriteValue((long)ms * 1000);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return Epoch.AddMilliseconds((long)reader.Value / 1000d);
        }
    }
}
