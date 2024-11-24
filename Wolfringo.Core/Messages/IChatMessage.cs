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
        /// <remarks>WOLF seems to prefer using <see cref="Timestamp"/> as message identifier now. Please use this to identify the message.</remarks>
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

    /// <summary>Represents a message that can have formatting metdatada attached to it</summary>
    public interface IFormattableMessage
    {
        /// <summary>The metadata for formatting of links in the message text.</summary>
        [JsonProperty("formatting", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        ChatMessageFormatting FormattingMetadata { get; }
    }
}
