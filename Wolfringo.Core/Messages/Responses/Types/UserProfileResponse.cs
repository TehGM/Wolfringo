using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class UserProfileResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        [JsonConverter(typeof(ExtractValuesOnlyConverter<WolfUser>))]
        public IEnumerable<WolfUser> UserProfiles { get; private set; }
    }
}
