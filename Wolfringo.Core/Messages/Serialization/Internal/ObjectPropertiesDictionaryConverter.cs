using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    public class ObjectPropertiesDictionaryConverter<TKey, TValue> : JsonConverter
    {
        private readonly string _keyPropName;
        private readonly string _valuePropName;

        public ObjectPropertiesDictionaryConverter(string keyPropertyName, string valuePropertyName)
        {
            this._keyPropName = keyPropertyName;
            this._valuePropName = valuePropertyName;
        }

        public ObjectPropertiesDictionaryConverter()
            : this("Key", "Value") { }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<KeyValuePair<TKey, TValue>>).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> collection = (IEnumerable<KeyValuePair<TKey, TValue>>)value;
            JArray results = new JArray();
            foreach (KeyValuePair<TKey, TValue> pair in collection)
            {
                results.Add(new JObject(
                    new JProperty(_keyPropName, JToken.FromObject(pair.Key, serializer)),
                    new JProperty(_valuePropName, JToken.FromObject(pair.Value, serializer))));
            }
            results.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jsonArray = JArray.Load(reader);
            Dictionary<TKey, TValue> results = new Dictionary<TKey, TValue>(jsonArray.Count);
            foreach (JToken obj in jsonArray)
            {
                TKey key = obj[_keyPropName].ToObject<TKey>(serializer);
                TValue value = obj[_valuePropName].ToObject<TValue>(serializer);
                results.Add(key, value);
            }
            return results;
        }
    }
}
