using Newtonsoft.Json.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for tipping message.</summary>
    /// <remarks>This special serializer moves "context" properties into "context" object to match the protocol.</remarks>
    public class TipAddMessageSerializer : DefaultMessageSerializer<TipAddMessage>
    {
        /// <inheritdoc/>
        public override IWolfMessage Deserialize(string command, SerializedMessageData messageData)
        {
            // deserialize message
            TipAddMessage result = (TipAddMessage)base.Deserialize(command, messageData);
            messageData.Payload.PopulateObject(result, "context", SerializationHelper.DefaultSerializer);
            messageData.Payload.PopulateObject(result, "body.context", SerializationHelper.DefaultSerializer);

            return result;
        }

        /// <inheritdoc/>
        public override SerializedMessageData Serialize(IWolfMessage message)
        {
            SerializedMessageData result = base.Serialize(message);
            JObject body = result.Payload["body"] as JObject;
            if (body == null)
                return result;
            // metadata props
            JObject context = new JObject();
            SerializationHelper.MovePropertyIfExists(ref body, ref context, "id");
            SerializationHelper.MovePropertyIfExists(ref body, ref context, "type");
            if (context.HasValues)
                body.Add(new JProperty("context", context));

            return result;
        }
    }
}
