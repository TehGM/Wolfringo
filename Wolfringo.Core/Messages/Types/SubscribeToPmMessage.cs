using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for subscribing to private messages.</summary>
    public class SubscribeToPmMessage : IHeadersWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.MessagePrivateSubscribe"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.MessagePrivateSubscribe;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };
    }
}
