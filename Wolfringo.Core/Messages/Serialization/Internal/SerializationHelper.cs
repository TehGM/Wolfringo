using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

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
            SerializerSettings.Converters.Add(new MicrosecondEpochConverter());
            SerializerSettings.Formatting = Formatting.None;

            DefaultSerializer = JsonSerializer.CreateDefault(SerializerSettings);
        }

        public static void PopulateObject<T>(this JToken token, ref T target, string childPath = null)
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
    }
}
