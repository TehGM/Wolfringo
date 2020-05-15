using Newtonsoft.Json;

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
    }
}
