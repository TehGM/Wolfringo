using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="GroupUpdateMessage"/> and <see cref="GroupCreateMessage"/>.</summary>
    public class GroupEditResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Updated or created group profile.</summary>
        [JsonProperty("body")]
        public WolfGroup GroupProfile { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected GroupEditResponse() : base() { }
    }
}
