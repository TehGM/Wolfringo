using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Utility class for selecting type of the response for a sent message.</summary>
    public interface IResponseTypeResolver
    {
        /// <summary>Gets response type for the message.</summary>
        /// <param name="messageType">Sent message type.</param>
        /// <param name="fallbackType">Type of response to use as fallback.</param>
        /// <returns>Type of the response.</returns>
        Type GetMessageResponseType(Type messageType, Type fallbackType = null);
    }
}
