using System;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for group edit responses.</summary>
    /// <remarks>This special serializer populates response's <see cref="WolfGroup"/> entity with the properties.</remarks>
    public class GroupEditResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _groupUpdateResponseType = typeof(GroupEditResponse);

        /// <inheritdoc/>
        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            GroupEditResponse result = (GroupEditResponse)base.Deserialize(responseType, responseData);
            // flatten props into group profile in addition to what the base class does
            GetResponseJson(responseData).FlattenCommonProperties(result.GroupProfile);
            return result;
        }

        /// <inheritdoc/>
        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_groupUpdateResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_groupUpdateResponseType.FullName}", nameof(responseType));
        }
    }
}
