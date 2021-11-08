using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for basic messages that don't have binary data.</summary>
    /// <typeparam name="T">Type of message for this serializer.</typeparam>
    public class DefaultMessageSerializer<T> : IMessageSerializer where T : IWolfMessage
    {
        /// <inheritdoc/>
        public virtual IWolfMessage Deserialize(string eventName, SerializedMessageData messageData)
        {
            IWolfMessage result = messageData.Payload.ToObject<T>(SerializationHelper.DefaultSerializer);
            messageData.Payload.FlattenCommonProperties(result, SerializationHelper.DefaultSerializer);
            return result;
        }

        /// <inheritdoc/>
        public virtual SerializedMessageData Serialize(IWolfMessage message)
            => new SerializedMessageData(message.SerializeJsonPayload(SerializationHelper.DefaultSerializer));
    }
}
