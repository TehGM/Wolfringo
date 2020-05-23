namespace TehGM.Wolfringo.Messages.Serialization
{
    public static class SerializerMapExtensions
    {
        public static TSerializer GetSerializer<TKey, TSerializer>(this ISerializerMap<TKey, TSerializer> map, TKey key)
        {
            if (map.TryFindMappedSerializer(key, out TSerializer result))
                return result;
            return map.FallbackSerializer;
        }

        public static bool TryFindMappedSerializer<TKey, TSerializer>(this ISerializerMap<TKey, TSerializer> map, TKey key, out TSerializer serializer)
        {
            serializer = map.FindMappedSerializer(key);
            return serializer != null;
        }
    }
}
