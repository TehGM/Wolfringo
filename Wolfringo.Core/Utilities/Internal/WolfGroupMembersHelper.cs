using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Utility class designed for handling WOLF protocol bug where group might not have members.</summary>
    public static class WolfGroupMembersHelper
    {
        /// <summary>Checks if group members are downloaded. If not, it'll attempt to repopulate it.</summary>
        /// <param name="client">Client to request group members with.</param>
        /// <param name="group">Group to validate members of.</param>
        /// <param name="cancellationToken">Token to cancel the task.</param>
        /// <returns>True if members list is valid after the operation; otherwise false.</returns>
        public static async Task<bool> RevalidateGroupMembersAsync(this IWolfClient client, WolfGroup group,
            CancellationToken cancellationToken = default)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (group.Members?.Any() == true)
                return true;

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            List<WolfGroupMember> retrievedMembers = new List<WolfGroupMember>();
            try
            {
                GroupMembersListResponse privilegedMembersResponse = await client.SendAsync<GroupMembersListResponse>(
                    new GroupMemberPrivilegedListMessage(group.ID, true), cancellationToken).ConfigureAwait(false);
                if (privilegedMembersResponse?.GroupMembers?.Any() == true)
                    retrievedMembers.AddRange(privilegedMembersResponse?.GroupMembers);


                const int limit = 100;
                uint lastMemberID = 0;

                for (; ; )
                {
                    GroupMembersListResponse regularMembersResponse = await client.SendAsync<GroupMembersListResponse>(
                        new GroupMemberRegularListMessage(group.ID, lastMemberID, limit, true), cancellationToken).ConfigureAwait(false);

                    int retrievedCount = regularMembersResponse?.GroupMembers?.Count() ?? 0;
                    if (retrievedCount > 0)
                    {
                        retrievedMembers.AddRange(regularMembersResponse.GroupMembers);
                        lastMemberID = retrievedMembers[retrievedMembers.Count - 1].UserID;
                    }

                    if (retrievedCount < limit)
                        break;
                }


                if (retrievedMembers.Count > 0)
                {
                    try
                    {
                        EntityModificationHelper.ReplaceAllGroupMembers(group, retrievedMembers);
                    }
                    catch (NotSupportedException) { return false; }
                    return true;
                }
            }
            // handle case when requesting profiles for group the user is not in
            catch (MessageSendingException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden) { }
            return false;
        }
    }
}
