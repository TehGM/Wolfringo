using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Utilities.Internal
{
    public static class EntityModificationHelper
    {
        //Log?.LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID);
        public static void ReplaceAllGroupMembers(WolfGroup group, IEnumerable<WolfGroupMember> members)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            IDictionary<uint, WolfGroupMember> collection = GetGroupMembersDictionary(group);
            collection.Clear();
            foreach (WolfGroupMember member in members)
                collection[member.UserID] = member;
        }

        public static void SetGroupMember(WolfGroup group, WolfGroupMember member)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            IDictionary<uint, WolfGroupMember> collection = GetGroupMembersDictionary(group);
            collection[member.UserID] = member;
        }

        public static void RemoveGroupMember(WolfGroup group, uint memberID)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            IDictionary<uint, WolfGroupMember> collection = GetGroupMembersDictionary(group);
            collection.Remove(memberID);
        }

        private static IDictionary<uint, WolfGroupMember> GetGroupMembersDictionary(WolfGroup group)
        {
            if (!(group.Members is IDictionary<uint, WolfGroupMember> membersDictionary) || membersDictionary.IsReadOnly)
                throw new NotSupportedException($"Members dictionary for group {group.ID} is read-only");
            return membersDictionary;
        }
    }
}
