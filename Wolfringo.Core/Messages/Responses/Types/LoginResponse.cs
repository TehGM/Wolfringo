using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="LoginMessage"/>.</summary>
    public class LoginResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Timestamp of first message received since gone offline.</summary>
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

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected LoginResponse() : base() { }

        /// <summary>Represents Amazon Web Services Cognito Identity.</summary>
        public class AwsCognitoIdentity
        {
            /// <summary>AWS Identity ID.</summary>
            [JsonProperty("identity")]
            public string Identity { get; private set; }
            /// <summary>AWS Identity auth token.</summary>
            [JsonProperty("token")]
            public string Token { get; private set; }
        }
    }
}
