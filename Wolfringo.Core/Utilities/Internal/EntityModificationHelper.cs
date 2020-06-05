using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Internal utility class for modifying entities state.</summary>
    /// <remarks>It's not recommended to use this class at all, unless it's required for writing a custom client or serializer implementation.</remarks>
    public static class EntityModificationHelper
    {
        /// <summary>Replaces al group members with provided entities.</summary>
        /// <remarks>It's not recommended to use this method at all, unless it's required for writing a custom client or serializer implementation.</remarks>
        /// <param name="group">Group to replace members of.</param>
        /// <param name="members">Members to set as group members.</param>
        /// <exception cref="ArgumentNullException"><paramref name="group"/> or <paramref name="members"/> is null.</exception>
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

        /// <summary>Updates one group member with provided entity.</summary>
        /// <remarks>It's not recommended to use this method at all, unless it's required for writing a custom client or serializer implementation.</remarks>
        /// <param name="group">Group to update group member in.</param>
        /// <param name="member">New member to add or update.</param>
        /// <exception cref="ArgumentNullException"><paramref name="group"/> or <paramref name="member"/> is null.</exception>
        public static void SetGroupMember(WolfGroup group, WolfGroupMember member)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            IDictionary<uint, WolfGroupMember> collection = GetGroupMembersDictionary(group);
            collection[member.UserID] = member;
        }

        /// <summary>Removes group member from group member list.</summary>
        /// <remarks>It's not recommended to use this method at all, unless it's required for writing a custom client or serializer implementation.</remarks>
        /// <param name="group">Group to remove member from.</param>
        /// <param name="memberID">User ID of member to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="group"/> is null.</exception>
        public static void RemoveGroupMember(WolfGroup group, uint memberID)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            IDictionary<uint, WolfGroupMember> collection = GetGroupMembersDictionary(group);
            collection.Remove(memberID);
        }

        /// <summary>Gets group members dictionary as editable.</summary>
        /// <param name="group">Group to extract members dictionary from.</param>
        /// <returns>Editable version of group members dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="group"/> is null.</exception>
        /// <exception cref="NotSupportedException">Group members dictionary is read only.</exception>
        private static IDictionary<uint, WolfGroupMember> GetGroupMembersDictionary(WolfGroup group)
        {
            if (!(group.Members is IDictionary<uint, WolfGroupMember> membersDictionary) || membersDictionary.IsReadOnly)
                throw new NotSupportedException($"Members dictionary for group {group.ID} is read-only");
            return membersDictionary;
        }
    }
}
