using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="ContactListMessage"/>.</summary>
    public class ContactListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>IDs of contacts.</summary>
        [JsonProperty("body", ItemConverterType = typeof(EntityIdConverter))]
        public IEnumerable<uint> ContactIDs { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected ContactListResponse() : base() { }
    }
}
