using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="UserUpdateMessage"/>.</summary>
    public class UserUpdateResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Updated user profile.</summary>
        [JsonProperty("body")]
        public WolfUser UserProfile { get; private set; }

        [JsonConstructor]
        protected UserUpdateResponse() : base() { }
    }
}
