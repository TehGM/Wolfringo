using Newtonsoft.Json;

namespace TehGM.Wolfringo
{
    /// <summary>Represents a Wolf entity.</summary>
    public interface IWolfEntity
    {
        /// <summary>ID of the entity.</summary>
        [JsonProperty("id")]
        uint ID { get; }
    }
}
