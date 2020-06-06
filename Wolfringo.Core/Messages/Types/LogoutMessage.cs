using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for logging out.</summary>
    public class LogoutMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SecurityLogout;
    }
}
