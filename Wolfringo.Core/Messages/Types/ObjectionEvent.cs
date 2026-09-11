using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Represents a WOLF objection error.</summary>
    public class ObjectionEvent : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.Objection"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.Objection;

        /// <summary>Error message.</summary>
        [JsonProperty("message")]
        public string Message { get; private set; }
        /// <summary>Time in seconds to wait before reconnecting.</summary>
        /// <remarks>Value of -1 indicates no reconnect.</remarks>
        [JsonProperty("reconnectSeconds")]
        public int ReconnectSeconds { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected ObjectionEvent() { }
    }
}
