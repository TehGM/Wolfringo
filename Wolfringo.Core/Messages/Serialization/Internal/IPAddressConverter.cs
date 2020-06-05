using Newtonsoft.Json;
using System;
using System.Net;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for converting IP Address.</summary>
    public class IPAddressConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }
}
