namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for serializing and deserializing Wolf messages and events.</summary>
    public interface IMessageSerializer
    {
        /// <summary>Serializes a message.</summary>
        /// <param name="message">Message to serialize.</param>
        /// <returns>Serialized message data.</returns>
        SerializedMessageData Serialize(IWolfMessage message);
        /// <summary>Deserializes a message.</summary>
        /// <param name="command">Message command.</param>
        /// <param name="messageData">Serialized message data.</param>
        /// <returns>Deserialized message.</returns>
        IWolfMessage Deserialize(string command, SerializedMessageData messageData);
    }
}
