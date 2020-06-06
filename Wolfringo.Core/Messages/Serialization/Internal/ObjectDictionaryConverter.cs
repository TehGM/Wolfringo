using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter converting Json object into dictionary value, and using one of it's properties as the key.</summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class ObjectDictionaryConverter<TKey, TValue> : JsonConverter
    {
        private readonly string _keyPropName;

        /// <inheritdoc/>
        /// <param name="keyPropertyName">Name of the property to use as a key.</param>
        public ObjectDictionaryConverter(string keyPropertyName)
        {
            this._keyPropName = keyPropertyName;
        }

        /// <inheritdoc/>
        public ObjectDictionaryConverter()
            : this("Key") { }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<KeyValuePair<TKey, TValue>>).IsAssignableFrom(objectType);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> collection = (IEnumerable<KeyValuePair<TKey, TValue>>)value;
            JArray results = new JArray();
            foreach (KeyValuePair<TKey, TValue> pair in collection)
            {
                JObject item = JObject.FromObject(pair.Value, serializer);
                item.Add(_keyPropName, JToken.FromObject(pair.Key, serializer));
                results.Add(item);
            }
            results.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jsonArray = JArray.Load(reader);
            Dictionary<TKey, TValue> results = new Dictionary<TKey, TValue>(jsonArray.Count);
            foreach (JToken obj in jsonArray)
            {
                TKey key = obj[_keyPropName].ToObject<TKey>(serializer);
                TValue value = obj.ToObject<TValue>(serializer);
                results.Add(key, value);
            }
            return results;
        }
    }
}
