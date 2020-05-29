using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    // https://stackoverflow.com/questions/19971494/how-to-deserialize-a-unix-timestamp-%CE%BCs-to-a-datetime-from-json/56795442

    public class EpochConverter : DateTimeConverterBase
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double ms = ((DateTime)value - Epoch).TotalMilliseconds;
            writer.WriteValue((long)ms);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return Epoch.AddMilliseconds((long)reader.Value / 1000d);
        }
    }
}
