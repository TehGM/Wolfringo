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
                {
                    results.Add(default);
                    continue;
                }

                string codeValue = GetCodeValue();
                if (int.TryParse(codeValue, out int code) && !(code >= 200 && code < 300))
                {
                    results.Add(default);
                    continue;
                }
                results.Add(GetResult());

                // if item is a JProperty, treat the collection as a dictionary
                // if it's a JObject, we're most likely dealing with an array
                // WOLF be inconsistent like that
                string GetCodeValue()
                {
                    if (item is JProperty)
                        return item.First["code"]?.Value<string>();
                    else
                        return item["code"]?.Value<string>();
                }

                T GetResult()
                {
                    T result;
                    if (item is JProperty)
                    {
                        result = item.First.ToObject<T>(serializer);
                        item.First.FlattenCommonProperties(result, serializer);
                    }
                    else
                    {
                        result = item.ToObject<T>(serializer);
                        item.FlattenCommonProperties(result, serializer);
                    }
                    return result;
                }
            }
            return results.ToArray();
        }
    }
}
