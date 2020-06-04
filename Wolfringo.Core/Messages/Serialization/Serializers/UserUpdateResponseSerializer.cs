using System;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class UserUpdateResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _userUpdateResponseType = typeof(UserUpdateResponse);

        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            UserUpdateResponse result = (UserUpdateResponse)base.Deserialize(responseType, responseData);
            // flatten props into user profile in addition to what the base class does
            GetResponseJson(responseData).FlattenCommonProperties(result.UserProfile);
            return result;
        }

        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_userUpdateResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_userUpdateResponseType.FullName}", nameof(responseType));
        }
    }
}
