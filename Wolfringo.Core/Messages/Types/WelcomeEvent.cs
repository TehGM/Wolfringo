using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Event when connection established.</summary>
    public class WelcomeEvent : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.Welcome;

        /// <summary>IP address connection is established from.</summary>
        [JsonProperty("ip")]
        public IPAddress IP { get; private set; }
        /// <summary>Country connection is established from.</summary>
        [JsonProperty("country")]
        public string CountryCode { get; private set; }
        /// <summary>Token for this connection.</summary>
        [JsonProperty("token")]
        public string Token { get; private set; }
        /// <summary>Wolf endpoint configuration.</summary>
        [JsonProperty("endpointConfig")]
        public EndpointConfig Endpoints { get; private set; }

        /// <summary>User if already logged in with provided token.</summary>
        [JsonProperty("loggedInUser")]
        public WolfUser LoggedInUser { get; private set; }

        /// <summary>Wolf endpoint configuration.</summary>
        public class EndpointConfig
        {
            /// <summary>Avatars endpoint.</summary>
            [JsonProperty("avatarEndpoint")]
            public string AvatarEndpoint { get; private set; }
            [JsonProperty("mmsUploadEndpoint")]
            public string MmsUploadEndpoint { get; private set; }
            /// <summary>Banner images endpoint.</summary>
            [JsonProperty("banner")]
            public BannerEndpointConfig BannerEndpoints { get; private set; }
        }

        /// <summary>Banner images endpoint.</summary>
        public class BannerEndpointConfig
        {
            /// <summary>Notification banner images endpoint.</summary>
            [JsonProperty("notification")]
            public IDictionary<string, string> NotificationEndpoints { get; private set; }
            /// <summary>Promotions banner images endpoint.</summary>
            [JsonProperty("promotion")]
            public IDictionary<string, string> PromotionEndpoints { get; private set; }
        }

        [JsonConstructor]
        private WelcomeEvent() { }
    }
}
