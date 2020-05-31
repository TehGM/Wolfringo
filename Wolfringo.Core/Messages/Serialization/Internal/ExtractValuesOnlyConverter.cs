using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    public class ExtractValuesOnlyConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (typeof(IEnumerable<T>).IsAssignableFrom(objectType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value, serializer);
            t.WriteTo(writer);
        }

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
                item.First.FlattenCommonProperties(result);
                results.Add(result);
            }
            return results.ToArray();
        }
    }
}
