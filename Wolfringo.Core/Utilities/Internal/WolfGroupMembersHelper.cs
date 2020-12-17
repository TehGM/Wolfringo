using System;
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
            try
            {
                GroupMembersListResponse membersResponse = await client.SendAsync<GroupMembersListResponse>(
                    new GroupMembersListMessage(group.ID), cancellationToken).ConfigureAwait(false);
                // client should be configured to intercept this response
                // however, just in case it's not (like when caching is disabled), do it here as well
                if (membersResponse?.GroupMembers?.Any() == true)
                {
                    try
                    {
                        EntityModificationHelper.ReplaceAllGroupMembers(group, membersResponse.GroupMembers);
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
