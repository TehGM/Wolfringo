using Newtonsoft.Json.Linq;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class UserUpdateMessageSerializer : DefaultMessageSerializer<UserUpdateMessage>
    {
        public override SerializedMessageData Serialize(IWolfMessage message)
        {
            SerializedMessageData result = base.Serialize(message);
            JObject body = result.Payload["body"] as JObject;
            if (body == null)
                return result;
            // extended props
            JObject extended = new JObject();
            MoveProperty(ref body, ref extended, "name");
            MoveProperty(ref body, ref extended, "about");
            MoveProperty(ref body, ref extended, "lookingFor");
            MoveProperty(ref body, ref extended, "gender");
            MoveProperty(ref body, ref extended, "relationship");
            MoveProperty(ref body, ref extended, "language");
            MoveProperty(ref body, ref extended, "urls");
            if (extended.HasValues)
                body.Add(new JProperty("extended", extended));
            return result;
        }


        private static void MoveProperty(ref JObject source, ref JObject target, string propertyName)
        {
            JToken value = source[propertyName];
            if (value == null)
                return;
            target.Add(new JProperty(propertyName, value));
            source.Remove(propertyName);
        }
    }
}
