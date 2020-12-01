using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>An event when group audio counts have changed.</summary>
    public class GroupAudioCountUpdateEvent : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupAudioCountUpdate"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupAudioCountUpdate;

        /// <summary>Group ID.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>New count of broadcasting members.</summary>
        [JsonProperty("broadcasterCount")]
        public int BroadcastersCount { get; private set; }
        /// <summary>New count of listening members.</summary>
        [JsonProperty("consumerCount")]
        public int ListenersCount { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupAudioCountUpdateEvent() { }
    }
}
