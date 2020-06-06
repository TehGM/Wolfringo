using System;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for user update responses.</summary>
    /// <remarks>This special serializer populates response's <see cref="WolfUser"/> entity with the properties.</remarks>
    public class UserUpdateResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _userUpdateResponseType = typeof(UserUpdateResponse);

        /// <inheritdoc/>
        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            UserUpdateResponse result = (UserUpdateResponse)base.Deserialize(responseType, responseData);
            // flatten props into user profile in addition to what the base class does
            GetResponseJson(responseData).FlattenCommonProperties(result.UserProfile);
            return result;
        }

        /// <inheritdoc/>
        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_userUpdateResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_userUpdateResponseType.FullName}", nameof(responseType));
        }
    }
}
