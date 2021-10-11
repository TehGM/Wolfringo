using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter using property names as keys and their nested properties as values.</summary>
    public class KeyAndValueDictionaryConverter<TKey, TValue> : JsonConverter
    {
        private readonly string _valuePropPath;

        /// <inheritdoc/>
        /// <param name="valuePropertyPath">Path of property to use as a value.</param>
        public KeyAndValueDictionaryConverter(string valuePropertyPath)
        {
            this._valuePropPath = valuePropertyPath;
        }

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
                results.AddAtPath($"{pair.Key}.{this._valuePropPath}", 
                    pair.Value != null ? JToken.FromObject(pair.Value, serializer) : null);
            }
            results.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObj = JObject.Load(reader);
            IEnumerable<JProperty> properties = jObj.Properties();
            Dictionary<TKey, TValue> results = new Dictionary<TKey, TValue>(properties.Count());
            TypeConverter keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            foreach (JProperty prop in properties)
            {
                TKey key = (TKey)keyConverter.ConvertFromInvariantString(prop.Name);
                TValue value = default;
                if (prop.Value != null)
                    value = prop.Value.SelectToken(this._valuePropPath).ToObject<TValue>(serializer);
                results.Add(key, value);
            }
            return results;
        }
    }
}
