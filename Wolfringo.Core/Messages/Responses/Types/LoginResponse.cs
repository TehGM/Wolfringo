using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="LoginMessage"/>.</summary>
    public class LoginResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("offlineMessageTimestamp")]
        public WolfTimestamp OfflineMessageTimestamp { get; private set; }

        /// <summary>Logged in user.</summary>
        [JsonProperty("subscriber")]
        public WolfUser User { get; private set; }
        /// <summary>Is this login a part of a new user registration?</summary>
        [JsonProperty("isNew")]
        public bool IsNewUser { get; private set; }
        /// <summary>Details on user in AWS Cognito.</summary>
        [JsonProperty("cognito")]
        public AwsCognitoIdentity AwsCognitoDetails { get; private set; }

        [JsonConstructor]
        protected LoginResponse() : base() { }

        public class AwsCognitoIdentity
        {
            [JsonProperty("identity")]
            public string Identity { get; private set; }
            [JsonProperty("token")]
            public string Token { get; private set; }
        }
    }
}
