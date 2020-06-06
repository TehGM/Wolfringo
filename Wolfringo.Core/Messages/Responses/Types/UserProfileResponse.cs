using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="UserProfileMessage"/>.</summary>
    public class UserProfileResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Users.</summary>
        [JsonProperty("body")]
        [JsonConverter(typeof(ExtractValuesOnlyConverter<WolfUser>))]
        public IEnumerable<WolfUser> UserProfiles { get; private set; }

        [JsonConstructor]
        protected UserProfileResponse() : base() { }
    }
}
