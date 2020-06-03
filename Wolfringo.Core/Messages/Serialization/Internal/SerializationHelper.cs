using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    public static class SerializationHelper
    {
        public static readonly JsonSerializerSettings SerializerSettings;
        public static readonly JsonSerializer DefaultSerializer;

        static SerializationHelper()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.Converters.Add(new IPAddressConverter());
            SerializerSettings.Converters.Add(new IPEndPointConverter());
            SerializerSettings.Formatting = Formatting.None;

            DefaultSerializer = JsonSerializer.CreateDefault(SerializerSettings);
        }

        public static void PopulateObject<T>(this JToken token, T target, string childPath = null)
        {
            JToken source = childPath != null ? token.SelectToken(childPath) : token;
            // sometimes body can be an array - if target is not an enumerable, ignore
            if (source is JArray && !(target is IEnumerable))
                return;
            if (source == null)
                return;
            using (JsonReader reader = source.CreateReader())
                SerializationHelper.DefaultSerializer.Populate(reader, target);
        }

        public static void FlattenCommonProperties<T>(this JToken token, T target)
        {
            token.PopulateObject(target, "body");
            token.PopulateObject(target, "headers");
            token.PopulateObject(target, "body.extended");
            token.PopulateObject(target, "body.base");
        }

        public static JObject SerializeJsonPayload<T>(this T message) where T : IWolfMessage
        {
            JObject payload = new JObject();
            JToken body = JToken.FromObject(message, SerializationHelper.DefaultSerializer);
            if (body.HasValues)
                payload.Add(new JProperty("body", body));
            if (message is IHeadersWolfMessage headersMessage && headersMessage.Headers?.Any() == true)
                payload.Add(new JProperty("headers", JToken.FromObject(headersMessage.Headers, SerializationHelper.DefaultSerializer)));
            return payload;
        }

        public static void MovePropertyIfExists(ref JObject source, ref JObject target, string propertyName)
        {
            if (!source.ContainsKey(propertyName))
                return;
            JToken value = source[propertyName];
            source.Remove(propertyName);
            target.Add(new JProperty(propertyName, value));
        }
    }
}
