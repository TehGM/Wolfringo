using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Represents an embedded visual data attached to a chat message.</summary>
    public interface IChatEmbed
    {
        /// <summary>Discriminator of given embed type.</summary>
        [JsonProperty("type")]
        string EmbedType { get; }
    }
}
