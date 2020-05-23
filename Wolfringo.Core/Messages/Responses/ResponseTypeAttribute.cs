using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Responses
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ResponseTypeAttribute : Attribute
    {
        public static readonly Type ParentResponseType = typeof(WolfResponse);
        public Type ResponseType { get; }

        public ResponseTypeAttribute(Type responseType)
            : base()
        {
            if (!ParentResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"Response type must inherit from {ParentResponseType.FullName}", nameof(responseType));

            this.ResponseType = responseType;
        }
    }
}
