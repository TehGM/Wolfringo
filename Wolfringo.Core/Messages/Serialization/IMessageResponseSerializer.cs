using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public interface IMessageResponseSerializer
    {
        WolfResponse Deserialize(Type responseType, SerializedMessageData responseData);
    }
}
