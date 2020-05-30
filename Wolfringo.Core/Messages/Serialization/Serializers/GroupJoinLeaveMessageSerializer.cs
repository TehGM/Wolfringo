using Newtonsoft.Json.Linq;
using System;

namespace TehGM.Wolfringo.Messages.Serialization
{
    // this protocol is stupid, and uses "groupId" for receiving and "id" for sending, while they're the same thing
    // this serializer will detect when the message is being sent, and replace "groupId" with "id"
    public class GroupJoinLeaveMessageSerializer<T> : DefaultMessageSerializer<T> where T : IWolfMessage
    {
        private static readonly Type _joinMessageType = typeof(GroupJoinMessage);
        private static readonly Type _leaveMessageType = typeof(GroupLeaveMessage);

        public GroupJoinLeaveMessageSerializer()
        {
            ThrowIfInvalidMessageType(typeof(T));
        }

        public override IWolfMessage Deserialize(string command, SerializedMessageData messageData)
        {
            if (messageData.Payload["body"]?["id"] == null)
                return base.Deserialize(command, messageData);
            else
            {
                JToken payload = messageData.Payload.DeepClone();
                JObject body = (JObject)payload["body"];
                JToken value = body["id"];
                body.Remove("id");
                body.Add("groupId", value);
                return base.Deserialize(command, new SerializedMessageData(payload, messageData.BinaryMessages));
            }
        }

        public override SerializedMessageData Serialize(IWolfMessage message)
        {
            ThrowIfInvalidMessageType(message.GetType());

            SerializedMessageData result = base.Serialize(message);
            // when sending, UserID will be null, so can use that to determine
            if ((message is GroupJoinMessage joinMessage && joinMessage.UserID == null) ||
                (message is GroupLeaveMessage leaveMessage && leaveMessage.UserID == null))
            {
                JObject body = (JObject)result.Payload["body"];
                JToken value = body["groupId"];
                body.Remove("groupId");
                body.Add("id", value);
            }
            return result;
        }

        private void ThrowIfInvalidMessageType(Type messageType)
        {
            if (!_joinMessageType.IsAssignableFrom(messageType) && !_leaveMessageType.IsAssignableFrom(messageType))
                throw new InvalidOperationException($"{this.GetType().Name} can only support {_joinMessageType.Name} and {_leaveMessageType.Name}");
        }
    }
}
