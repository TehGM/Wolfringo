using Newtonsoft.Json.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for group edit message, such as group creation or update.</summary>
    /// <remarks>This special serializer moves "extended" properties into "extended" object to match the protocol.</remarks>
    /// <typeparam name="T">Type of group edit message.</typeparam>
    public class GroupEditMessageSerializer<T> : DefaultMessageSerializer<T> where T : IGroupEditMessage, IWolfMessage
    {
        /// <inheritdoc/>
        public override SerializedMessageData Serialize(IWolfMessage message)
        {
            SerializedMessageData result = base.Serialize(message);
            JObject body = result.Payload["body"] as JObject;
            if (body == null)
                return result;
            // extended props
            JObject extended = new JObject();
            SerializationHelper.MovePropertyIfExists(ref body, ref extended, "advancedAdmin");
            SerializationHelper.MovePropertyIfExists(ref body, ref extended, "discoverable");
            SerializationHelper.MovePropertyIfExists(ref body, ref extended, "entryLevel");
            SerializationHelper.MovePropertyIfExists(ref body, ref extended, "language");
            SerializationHelper.MovePropertyIfExists(ref body, ref extended, "longDescription");
            if (extended.HasValues)
                body.Add(new JProperty("extended", extended));
            return result;
        }
    }
}
