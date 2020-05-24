using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    public static class ResponseTypeResolverExtensions
    {
        public static Type GetMessageResponseType<TFallbackType>(this IResponseTypeResolver resolver, IWolfMessage message) where TFallbackType : IWolfResponse
            => resolver.GetMessageResponseType(message.GetType(), typeof(TFallbackType));
    }
}
