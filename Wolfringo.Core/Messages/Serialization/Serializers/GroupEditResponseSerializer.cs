using System;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class GroupEditResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _groupUpdateResponseType = typeof(GroupEditResponse);

        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            GroupEditResponse result = (GroupEditResponse)base.Deserialize(responseType, responseData);
            // flatten props into user profile in addition to what the base class does
            GetResponseJson(responseData).FlattenCommonProperties(result.GroupProfile);
            return result;
        }

        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_groupUpdateResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_groupUpdateResponseType.FullName}", nameof(responseType));
        }
    }
}
