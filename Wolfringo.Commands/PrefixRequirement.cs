using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Specifies when prefix is required at the beginning of the message.</summary>
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PrefixRequirement
    {
        /// <summary>Prefix is not required.</summary>
        Never = 0,
        /// <summary>Prefix is required in group messages.</summary>
        Group = 1 << 0,
        /// <summary>Prefix is required in private messages.</summary>
        Private = 1 << 1,
        /// <summary>Prefix is required in all message types.</summary>
        Always = Group | Private
    }
}
