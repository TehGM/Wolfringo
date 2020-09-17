using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for user update responses.</summary>
    /// <remarks>This special serializer changes the inconsistent charm ID property name when deserializing.</remarks>
    public class TipDetailsResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _tipDetailsResponseType = typeof(TipDetailsResponse);

        /// <inheritdoc/>
        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            // so, in this abomination of a protocol, tip charmID is serialized as "id" in most cases...
            // ... except when retrieving detailed info on message tips, where it's serialized as "charmId"
            // even though summaries for the same thing call it "id"
            // yes... fixing inconsistencies YET AGAIN. Who designed this protocol?!
            JToken payload = GetResponseJson(responseData.Payload).DeepClone();
            JArray charmList = payload.SelectToken("body.list") as JArray;
            foreach (JObject charmDetails in charmList.Children().Cast<JObject>())
            {
                JToken value = charmDetails["charmId"];
                if (value != null)
                {
                    charmDetails.Remove("charmId");
                    charmDetails.Add("id", value);
                }
            }
            return (TipDetailsResponse)base.Deserialize(responseType, new SerializedMessageData(payload, responseData.BinaryMessages));
        }

        /// <inheritdoc/>
        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_tipDetailsResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_tipDetailsResponseType.FullName}", nameof(responseType));
        }
    }
}