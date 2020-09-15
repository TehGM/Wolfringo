using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Represents a message that appears in the chat.</summary>
    [ResponseType(typeof(ChatResponse))]
    public interface IChatMessage : IWolfMessage, IRawDataMessage
    {
        // json data
        [JsonProperty("flightId", NullValueHandling = NullValueHandling.Ignore)]
        string FlightID { get; }
        /// <summary>Unique ID of the message.</summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        [Obsolete("WOLF protocol now prefers to use Timestamp as a message ID.")]
        Guid? ID { get; }
        /// <summary>Is it a group message?</summary>
        [JsonProperty("isGroup")]
        bool IsGroupMessage { get; }
        /// <summary>Type of the message.</summary>
        [JsonProperty("mimeType")]
        string MimeType { get; }
        /// <summary>Message's timestamp.</summary>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MillisecondsEpochConverter))]
        DateTime? Timestamp { get; }
        /// <summary>User that sent the message.</summary>
        [JsonProperty("originator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EntityIdConverter))]
        uint? SenderID { get; }
        /// <summary>User or group that received the message.</summary>
        [JsonProperty("recipient")]
        [JsonConverter(typeof(EntityIdConverter))]
        uint RecipientID { get; }
    }

    public interface IRawDataMessage
    {
        // binary data
        /// <summary>Message's raw binary data.</summary>
        [JsonIgnore]
        IReadOnlyCollection<byte> RawData { get; }
    }
}
