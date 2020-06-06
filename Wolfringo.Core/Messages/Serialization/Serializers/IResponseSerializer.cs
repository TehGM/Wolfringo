using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for deserializing Wolf server responses.</summary>
    public interface IResponseSerializer
    {
        /// <summary>Deserialize a server response.</summary>
        /// <param name="responseType">Type of the response to deserialize into.</param>
        /// <param name="responseData">Serialized response data.</param>
        /// <returns>Deserialized response.</returns>
        IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData);
    }
}
