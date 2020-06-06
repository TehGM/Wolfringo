using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for subscribing to private messages.</summary>
    public class SubscribeToPmMessage : IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.MessagePrivateSubscribe;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };
    }
}
