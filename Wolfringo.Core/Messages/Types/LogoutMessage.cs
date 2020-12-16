using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for logging out.</summary>
    public class LogoutMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.SecurityLogout"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SecurityLogout;
    }
}
