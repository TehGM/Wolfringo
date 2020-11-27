namespace TehGM.Wolfringo.Messages.Serialization
{
    public static class SerializerProviderExtensions
    {
        /// <summary>Gets serializer mapped to the key. If not found, will return fallback serializer.</summary>
        /// <typeparam name="TKey">Type of the serializer key.</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
        /// <param name="key">Key to get the serializer for.</param>
        /// <returns>Found serializer; if not found, then fallback serializer</returns>
        public static TSerializer GetSerializer<TKey, TSerializer>(this ISerializerProvider<TKey, TSerializer> map, TKey key)
        {
            if (map.TryFindMappedSerializer(key, out TSerializer result))
                return result;
            return map.FallbackSerializer;
        }

        /// <summary>Gets serializer mapped to the key.</summary>
        /// <typeparam name="TKey">Type of the serializer key.</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
        /// <param name="key">Key to get the serializer for.</param>
        /// <param name="serializer">Found serializer.</param>
        /// <returns>True if non-fallback serializer was found; otherwise false.</returns>
        public static bool TryFindMappedSerializer<TKey, TSerializer>(this ISerializerProvider<TKey, TSerializer> map, TKey key, out TSerializer serializer)
        {
            serializer = map.GetSerializer(key);
            return serializer != null;
        }
    }
}
