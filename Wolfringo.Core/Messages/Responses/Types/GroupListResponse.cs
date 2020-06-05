using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="GroupListMessage"/>.</summary>
    public class GroupListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>IDs of groups the user is in.</summary>
        [JsonProperty("body", ItemConverterType = typeof(EntityIdConverter))]
        public IEnumerable<uint> UserGroupIDs { get; private set; }
    }
}
