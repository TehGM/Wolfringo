using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ResponseTypeAttribute : Attribute
    {
        public static readonly Type BaseResponseType = typeof(IWolfResponse);
        public Type ResponseType { get; }

        public ResponseTypeAttribute(Type responseType)
            : base()
        {
            if (!BaseResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"Response type must implement {BaseResponseType.FullName}", nameof(responseType));

            this.ResponseType = responseType;
        }
    }
}
