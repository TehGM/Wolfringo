using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Internal utilities for serialization.</summary>
    /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
    public static class SerializationHelper
    {
        /// <summary>Default serializer settings.</summary>
        public static readonly JsonSerializerSettings SerializerSettings;
        /// <summary>Default serializer.</summary>
        public static readonly JsonSerializer DefaultSerializer;

        static SerializationHelper()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.Converters.Add(new IPAddressConverter());
            SerializerSettings.Converters.Add(new IPEndPointConverter());
            SerializerSettings.Formatting = Formatting.None;

            DefaultSerializer = JsonSerializer.CreateDefault(SerializerSettings);
        }

        /// <summary>Populates object with Json token's properties.</summary>
        /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="token">Token to use for populating.</param>
        /// <param name="target">Object to populate.</param>
        /// <param name="childPath">Selector of the child token in the <paramref name="token"/>. 
        /// If null, <paramref name="token"/> will be used directly.</param>
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

        /// <summary>Populates object by "flattening" common properties like "body" or "body.extended" into target object.</summary>
        /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="token">Token to use for populating.</param>
        /// <param name="target">Object to populate.</param>
        public static void FlattenCommonProperties<T>(this JToken token, T target)
        {
            token.PopulateObject(target, "body");
            token.PopulateObject(target, "headers");
            token.PopulateObject(target, "body.extended");
            token.PopulateObject(target, "body.base");
        }

        /// <summary>Serializes Wolf message.</summary>
        /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
        /// <typeparam name="T">Type of the message.</typeparam>
        /// <param name="message">Message to serialize.</param>
        /// <returns>Serialized message object.</returns>
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

        /// <summary>Moves json property from one object to another.</summary>
        /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
        /// <param name="source">Object to remove the property from.</param>
        /// <param name="target">Object to add the property to.</param>
        /// <param name="propertyName">Name of the property.</param>
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
