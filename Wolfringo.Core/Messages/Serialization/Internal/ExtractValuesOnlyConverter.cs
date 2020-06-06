using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for deserializing only values of Json dictionaries.</summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class ExtractValuesOnlyConverter<T> : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return (typeof(IEnumerable<T>).IsAssignableFrom(objectType));
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value, serializer);
            t.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jsonObject = JToken.Load(reader);
            if (jsonObject == null)
                return default;
            List<T> results = new List<T>(jsonObject.Count());
            foreach (JToken item in jsonObject)
            {
                if (item == null || item.First == null)
                    results.Add(default);
                T result = item.First.ToObject<T>(serializer);
                item.First.FlattenCommonProperties(result, serializer);
                results.Add(result);
            }
            return results.ToArray();
        }
    }
}
