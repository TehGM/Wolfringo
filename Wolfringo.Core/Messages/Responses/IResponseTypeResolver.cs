using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    public interface IResponseTypeResolver
    {
        Type GetMessageResponseType(Type messageType, Type fallbackType = null);
    }
}
