using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <param name="childPath">Selector of the child token in the <paramref name="token"/>.</param>
        /// <param name="serializer">Serializer to use. If null, <see cref="DefaultSerializer"/> will be used.</param>
        public static void PopulateObject<T>(this JToken token, T target, string childPath = null, JsonSerializer serializer = null)
        {
            JToken source = childPath != null ? token.SelectToken(childPath) : token;
            // sometimes body can be an array - if target is not an enumerable, ignore
            if (source is JArray && !(target is IEnumerable))
                return;
            if (source == null)
                return;
            using (JsonReader reader = source.CreateReader())
            {
                if (serializer == null)
                    SerializationHelper.DefaultSerializer.Populate(reader, target);
                else
                    serializer.Populate(reader, target);
            }
        }

        /// <summary>Populates object by "flattening" common properties like "body" or "body.extended" into target object.</summary>
        /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="token">Token to use for populating.</param>
        /// <param name="target">Object to populate.</param>
        /// <param name="serializer">Serializer to use. If null, <see cref="DefaultSerializer"/> will be used.</param>
        public static void FlattenCommonProperties<T>(this JToken token, T target, JsonSerializer serializer = null)
        {
            token.PopulateObject(target, "body", serializer);
            token.PopulateObject(target, "headers", serializer);
            token.PopulateObject(target, "body.extended", serializer);
            token.PopulateObject(target, "body.base", serializer);
            token.PopulateObject(target, "metadata", serializer);
            token.PopulateObject(target, "body.metadata", serializer);
        }

        /// <summary>Serializes Wolf message.</summary>
        /// <remarks>It's not recommended to use this class unless it's required for writing a custom serializer implementation.</remarks>
        /// <typeparam name="T">Type of the message.</typeparam>
        /// <param name="message">Message to serialize.</param>
        /// <param name="serializer">Serializer to use. If null, <see cref="DefaultSerializer"/> will be used.</param>
        /// <returns>Serialized message object.</returns>
        public static JObject SerializeJsonPayload<T>(this T message, JsonSerializer serializer = null) where T : IWolfMessage
        {
            JObject payload = new JObject();
            JToken body = JToken.FromObject(message, serializer ?? SerializationHelper.DefaultSerializer);
            if (body.HasValues)
                payload.Add(new JProperty("body", body));
            if (message is IHeadersWolfMessage headersMessage && headersMessage.Headers?.Any() == true)
                payload.Add(new JProperty("headers", JToken.FromObject(headersMessage.Headers, serializer ?? SerializationHelper.DefaultSerializer)));
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

        /// <summary>Adds a new JProperty at given JsonPath. Note that only dot-notated child syntax is currently supported.</summary>
        /// <param name="targetObject">The <see cref="JObject"/> to add property to.</param>
        /// <param name="jsonPath">The JsonPath of the new property.</param>
        /// <param name="content">The property content.</param>
        /// <returns>The created <see cref="JProperty"/>.</returns>
        public static JProperty AddAtPath(this JObject targetObject, string jsonPath, object content)
            => targetObject.AddAtPath(jsonPath.Split('.'), content);

        private static JProperty AddAtPath(this JObject targetObject, IEnumerable<string> jsonPath, object content)
        {
            // fail case - property name is required
            if (jsonPath?.Any() != true)
                throw new ArgumentException("At least one path segment is required.", nameof(jsonPath));

            int segmentsCount = jsonPath.Count();
            string name = jsonPath.First();
            JProperty result;
            // simple case - no further nesting, just create
            if (segmentsCount == 1)
            {
                result = new JProperty(name, content);
                targetObject.Add(result);
                return result;
            }

            // annoying case - there's some nesting. Use recursion
            // we need to find what already exists in object, to not try to create doubled properties
            JObject foundSegment = targetObject[name] as JObject;
            if (foundSegment != null)
                return AddAtPath(foundSegment, jsonPath.Skip(1), content);

            // if it's a new segment and not final prop, create new object
            if (segmentsCount > 1)
            {
                JObject newSegment = new JObject();
                result = AddAtPath(newSegment, jsonPath.Skip(1), content);
                targetObject.Add(name, newSegment);
                return result;
            }

            // if it's final segment, finally create the property
            result = new JProperty(name, content);
            targetObject.Add(result);
            return result;
        }

        /// <summary>Populates chat message's raw data.</summary>
        /// <typeparam name="T">Type of chat message.</typeparam>
        /// <param name="message">Chat message.</param>
        /// <param name="data">Binary data.</param>
        public static void PopulateMessageRawData<T>(ref T message, IEnumerable<byte> data) where T : IRawDataMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Any() == true && data.First() == 4)
                data = data.Skip(1);


            if (message.RawData == null || !(message.RawData is ICollection<byte> byteCollection) || byteCollection.IsReadOnly)
                throw new InvalidOperationException($"Cannot populate raw data in {message.GetType().Name} as the collection is read only or null");
            byteCollection.Clear();
            // if it's a list, we can do it in a more performant way
            if (message.RawData is List<byte> byteList)
                byteList.AddRange(data);
            // otherwise do it one by one
            else
            {
                foreach (byte b in data)
                    byteCollection.Add(b);
            }
        }
    }
}
