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
        /// <summary>WOLF protocol internal flight ID.</summary>
        [JsonProperty("flightId", NullValueHandling = NullValueHandling.Ignore)]
        string FlightID { get; }
        /// <summary>Unique ID of the message.</summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        Guid? ID { get; }
        /// <summary>Is it a group message?</summary>
        [JsonProperty("isGroup")]
        bool IsGroupMessage { get; }
        /// <summary>Type of the message.</summary>
        [JsonProperty("mimeType")]
        string MimeType { get; }
        /// <summary>Message's timestamp.</summary>
        /// <remarks><para>When creating a new chat message, this value will be null. Once WOLF server acknowledges the message, it'll respond with timestamp value. 
        /// Default <see cref="WolfClient"/> implementation will automatically populate this value once this happens, so message will have timestamp 
        /// populated after <see cref="IWolfClient.SendAsync{TResponse}(IWolfMessage, System.Threading.CancellationToken)"/> returns.</para>
        /// <para>For received messages, this value will be populated normally.</para></remarks>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        WolfTimestamp? Timestamp { get; }
        /// <summary>User that sent the message.</summary>
        /// <remarks><para>When creating a new chat message, this value will be null.</para>
        /// <para>For received messages, this value will be populated normally.</para></remarks>
        [JsonProperty("originator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EntityIdConverter))]
        uint? SenderID { get; }
        /// <summary>User or group that received the message.</summary>
        [JsonProperty("recipient")]
        [JsonConverter(typeof(EntityIdConverter))]
        uint RecipientID { get; }
    }

    /// <summary>Represents a message containing raw binary data.</summary>
    public interface IRawDataMessage
    {
        // binary data
        /// <summary>Message's raw binary data.</summary>
        [JsonIgnore]
        IReadOnlyCollection<byte> RawData { get; }
    }
}
