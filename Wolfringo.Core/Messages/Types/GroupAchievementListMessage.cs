using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting groups's achievements list.</summary>
    /// <remarks>Uses <see cref="EntityAchievementListResponse"/> as response type.</remarks>
    [ResponseType(typeof(EntityAchievementListResponse))]
    public class GroupAchievementListMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.AchievementSubscriberList"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.AchievementSubscriberList;

        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };

        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupAchievementListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to get achievements of.</param>
        public GroupAchievementListMessage(uint groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}