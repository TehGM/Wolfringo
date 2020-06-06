using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo
{
    /// <summary>Represents a wolf command message or event.</summary>
    public interface IWolfMessage
    {
        /// <summary>Message command.</summary>
        [JsonIgnore]
        string Command { get; }
    }

    /// <summary>Represents a wolf command message or event that uses headers in payload.</summary>
    public interface IHeadersWolfMessage : IWolfMessage
    {
        /// <summary>Message payload headers.</summary>
        [JsonIgnore]
        IDictionary<string, object> Headers { get; }
    }
}
