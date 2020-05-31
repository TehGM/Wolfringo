using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for basic messages that don't have binary data.</summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultMessageSerializer<T> : IMessageSerializer where T : IWolfMessage
    {
        public virtual IWolfMessage Deserialize(string command, SerializedMessageData messageData)
        {
            IWolfMessage result = messageData.Payload.ToObject<T>(SerializationHelper.DefaultSerializer);
            messageData.Payload.FlattenCommonProperties(result);
            return result;
        }

        public virtual SerializedMessageData Serialize(IWolfMessage message)
            => new SerializedMessageData(message.SerializeJsonPayload());
    }
}
