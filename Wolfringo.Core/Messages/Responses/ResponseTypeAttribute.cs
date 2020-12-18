using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Sets preferred type of response for a message.</summary>
    /// <remarks>This attribute is honoured by <see cref="ResponseTypeResolver"/>. The default resolver will 
    /// return type set by this attribute, regardless of the response type requested by user when sending the message.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ResponseTypeAttribute : Attribute
    {
        /// <summary>Base type of all Wolf Responses.</summary>
        /// <remarks>Equals to <see cref="IWolfResponse"/> type.</remarks>
        public static readonly Type BaseResponseType = typeof(IWolfResponse);
        /// <summary>Preferred type of the response.</summary>
        public Type ResponseType { get; }

        /// <summary>Sets preferred type of response for a message.</summary>
        /// <param name="responseType">Response type for the message. Must implement <see cref="IWolfResponse"/> in it's inheritance chain.</param>
        public ResponseTypeAttribute(Type responseType)
            : base()
        {
            if (!BaseResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"Response type must implement {BaseResponseType.FullName}", nameof(responseType));

            this.ResponseType = responseType;
        }
    }
}
