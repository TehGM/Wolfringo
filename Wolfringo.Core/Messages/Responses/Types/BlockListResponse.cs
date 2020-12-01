using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="BlockListMessage"/>.</summary>
    public class BlockListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>IDs of blocked users.</summary>
        [JsonProperty("body", ItemConverterType = typeof(EntityIdConverter))]
        public IEnumerable<uint> BlockedUsersIDs { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected BlockListResponse() : base() { }
    }
}
