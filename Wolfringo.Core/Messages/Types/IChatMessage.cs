using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    public interface IChatMessage : IWolfMessage
    {
        // json data
        [JsonProperty("flightId", NullValueHandling = NullValueHandling.Ignore)]
        string FlightID { get; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        Guid? ID { get; }
        [JsonProperty("isGroup")]
        bool IsGroupMessage { get; }
        [JsonProperty("mimeType")]
        string MimeType { get; }
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EpochConverter))]
        DateTime? Timestamp { get; }
        [JsonProperty("originator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EntityIdConverter))]
        uint? SenderID { get; }
        [JsonProperty("recipient")]
        [JsonConverter(typeof(EntityIdConverter))]
        uint RecipientID { get; }

        // binary data
        [JsonIgnore]
        IReadOnlyCollection<byte> RawData { get; }
    }
}
