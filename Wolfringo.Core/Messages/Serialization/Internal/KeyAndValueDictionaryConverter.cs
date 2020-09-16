using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter using property names as keys and their nested properties as values.</summary>
    public class KeyAndValueDictionaryConverter<TKey, TValue> : JsonConverter
    {
        private readonly string _valuePropName;
        private readonly bool _skipInnerBody;

        /// <inheritdoc/>
        /// <param name="valuePropertyName">Name of property to use as a value.</param>
        /// <param name="skipInnerBody">Whether inner body message should be ignored, and only its content taken.</param>
        public KeyAndValueDictionaryConverter(string valuePropertyName, bool skipInnerBody)
        {
            this._valuePropName = valuePropertyName;
            this._skipInnerBody = skipInnerBody;
        }

        /// <inheritdoc/>
        /// <param name="valuePropertyName">Name of property to use as a value.</param>
        public KeyAndValueDictionaryConverter(string valuePropertyName)
            : this(valuePropertyName, true) { }

        /// <inheritdoc/>
        public KeyAndValueDictionaryConverter()
            : this("body") { }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<KeyValuePair<TKey, TValue>>).IsAssignableFrom(objectType);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> collection = (IEnumerable<KeyValuePair<TKey, TValue>>)value;
            JObject results = new JObject();
            foreach (KeyValuePair<TKey, TValue> pair in collection)
            {
                results.Add(new JProperty(pair.Key.ToString(),
                    new JProperty(_valuePropName, JToken.FromObject(pair.Value, serializer))));
            }
            results.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObj = JObject.Load(reader);
            IEnumerable<JProperty> properties = jObj.Properties();
            Dictionary<TKey, TValue> results = new Dictionary<TKey, TValue>(properties.Count());
            foreach (JProperty prop in properties)
            {
                TKey key = (TKey)Convert.ChangeType(prop.Name, typeof(TKey));
                TValue value = default;
                if (prop.Value != null)
                {
                    // if not skipping inner body, attempt to take that as property. If missing body, or skipping body, take the prop itself
                    JToken p = _skipInnerBody ? (prop.Value["body"] ?? prop.Value) : prop.Value;
                    value = p[_valuePropName].ToObject<TValue>(serializer);
                }
                results.Add(key, value);
            }
            return results;
        }
    }
}
