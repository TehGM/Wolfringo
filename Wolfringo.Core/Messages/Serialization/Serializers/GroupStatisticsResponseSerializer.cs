using System;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for group statistics responses.</summary>
    /// <remarks>This special serializer populates response's <see cref="WolfGroupStatistics"/> entity with the properties in details object.</remarks>
    public class GroupStatisticsResponseSerializer : DefaultResponseSerializer
    {
        private static readonly Type _groupStatsResponseType = typeof(GroupStatisticsResponse);

        /// <inheritdoc/>
        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            GroupStatisticsResponse result = (GroupStatisticsResponse)base.Deserialize(responseType, responseData);
            GetResponseJson(responseData).PopulateObject(result.GroupStatistics, "body.details");
            return result;
        }

        /// <inheritdoc/>
        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_groupStatsResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_groupStatsResponseType.FullName}", nameof(responseType));
        }
    }
}
