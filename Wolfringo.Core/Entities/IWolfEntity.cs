using Newtonsoft.Json;

namespace TehGM.Wolfringo
{
    public interface IWolfEntity
    {
        [JsonProperty("id")]
        uint ID { get; }
    }
}
