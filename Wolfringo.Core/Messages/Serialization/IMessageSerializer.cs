namespace TehGM.Wolfringo.Messages.Serialization
{
    public interface IMessageSerializer
    {
        SerializedMessageData Serialize(IWolfMessage message);
        IWolfMessage Deserialize(string command, SerializedMessageData messageData);
    }
}
