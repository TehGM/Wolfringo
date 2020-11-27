namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Utility class for mapping serializers.</summary>
    /// <typeparam name="TKey">Type of the serializer key.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    public interface ISerializerProvider<TKey, TSerializer>
    {
        /// <summary>Fallback serializer that can be used if key has no mapped serializer.</summary>
        TSerializer FallbackSerializer { get; }
        /// <summary>Gets serializer mapped to the key.</summary>
        /// <param name="key">Key to get the serializer for.</param>
        /// <returns>Found serializer.</returns>
        TSerializer GetSerializer(TKey key);
    }
}
