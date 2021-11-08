using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter using 2 properties of json object as dictionary key and value.</summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class ObjectPropertiesDictionaryConverter<TKey, TValue> : JsonConverter
    {
        private readonly string _keyPropPath;
        private readonly string _valuePropPath;

        /// <inheritdoc/>
        /// <param name="keyPropertyPath">Path of property to use as a key. Must </param>
        /// <param name="valuePropertyPath">Path of property to use as a value.</param>
        public ObjectPropertiesDictionaryConverter(string keyPropertyPath, string valuePropertyPath)
        {
            this._keyPropPath = keyPropertyPath;
            this._valuePropPath = valuePropertyPath;
        }

        /// <inheritdoc/>
        public ObjectPropertiesDictionaryConverter()
            : this("Key", "Value") { }

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
                JObject item = new JObject();
                item.AddAtPath(this._keyPropPath, JToken.FromObject(pair.Key, serializer));
                item.AddAtPath(this._valuePropPath, pair.Value != null ? JToken.FromObject(pair.Value, serializer) : null);
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
                TKey key = obj.SelectToken(this._keyPropPath).ToObject<TKey>(serializer);
                TValue value = obj.SelectToken(this._valuePropPath).ToObject<TValue>(serializer);
                results.Add(key, value);
            }
            return results;
        }
    }
}
