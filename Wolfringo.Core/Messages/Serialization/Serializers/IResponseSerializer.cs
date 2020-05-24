using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public interface IResponseSerializer
    {
        IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData);
    }
}
