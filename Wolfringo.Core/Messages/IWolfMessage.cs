using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo
{
    public interface IWolfMessage
    {
        [JsonIgnore]
        string Command { get; }
    }

    public interface IHeadersWolfMessage : IWolfMessage
    {
        [JsonIgnore]
        IDictionary<string, object> Headers { get; }
    }
}
