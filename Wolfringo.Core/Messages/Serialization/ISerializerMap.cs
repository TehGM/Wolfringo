namespace TehGM.Wolfringo.Messages.Serialization
{
    public interface ISerializerMap<TKey, TSerializer>
    {
        TSerializer FallbackSerializer { get; set; }
        TSerializer FindMappedSerializer(TKey key);
        void MapSerializer(TKey key, TSerializer serializer);
    }
}
