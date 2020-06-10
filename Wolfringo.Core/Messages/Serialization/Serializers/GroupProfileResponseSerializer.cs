using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class GroupProfileResponseSerializer : DefaultResponseSerializer
    {
        private static readonly Type _groupProfileResponseType = typeof(GroupProfileResponse);

        /// <inheritdoc/>
        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            // if body contains an object that contains yet another body, means it's nested, so treat it as normal
            // otherwise, it's just one group, and we need to nest it deeper so this serializer works
            // yes. This protocol is damn stupid. "What is consistency? We don't know, unless it's consistently bad!"
            SerializedMessageData data = responseData;
            IEnumerable<JToken> nestedGroupBodies = GetResponseJson(responseData).SelectTokens("body.*.body");
            if (nestedGroupBodies?.Any() != true)
            {
                JToken newJson = responseData.Payload.DeepClone();
                JObject newBody = GetResponseJson(newJson).SelectToken("body") as JObject;
                JEnumerable<JToken> children = newBody.Children();
                JObject groupBody = new JObject();
                foreach (JToken obj in children)
                    groupBody.Add(obj);
                newBody.RemoveAll();
                newBody.Add(new JProperty("0", new JObject(new JProperty("body", groupBody))));
                data = new SerializedMessageData(newJson, responseData.BinaryMessages);
            }

            GroupProfileResponse result = (GroupProfileResponse)base.Deserialize(responseType, data);
            return result;
        }

        /// <inheritdoc/>
        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_groupProfileResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_groupProfileResponseType.FullName}", nameof(responseType));
        }
    }
}
