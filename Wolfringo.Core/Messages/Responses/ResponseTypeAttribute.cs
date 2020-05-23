using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Responses
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class ResponseTypeAttribute : Attribute
    {
        public static readonly Type BaseResponseType = typeof(WolfResponse);
        public Type ResponseType { get; }

        public ResponseTypeAttribute(Type responseType)
            : base()
        {
            if (!BaseResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"Response type must inherit from {BaseResponseType.FullName}", nameof(responseType));

            this.ResponseType = responseType;
        }
    }
}
