using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace TehGM.Wolfringo.Messages
{
    public class WelcomeMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.Welcome;

        [JsonProperty("ip")]
        public IPAddress IP { get; private set; }
        [JsonProperty("country")]
        public string CountryCode { get; private set; }
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("endpointConfig")]
        public EndpointConfig Endpoints { get; private set; }

        public class EndpointConfig
        {
            [JsonProperty("avatarEndpoint")]
            public string AvatarEndpoint { get; private set; }
            [JsonProperty("mmsUploadEndpoint")]
            public string MmsUploadEndpoint { get; private set; }
            [JsonProperty("banner")]
            public BannerEndpointConfig BannerEndpoints { get; private set; }
        }

        public class BannerEndpointConfig
        {
            [JsonProperty("notification")]
            public IDictionary<string, string> NotificationEndpoints { get; private set; }
            [JsonProperty("promotion")]
            public IDictionary<string, string> PromotionEndpoints { get; private set; }
        }
    }
}
