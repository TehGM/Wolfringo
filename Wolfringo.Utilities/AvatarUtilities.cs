using System.Net.Http;

namespace TehGM.Wolfringo
{
    /// <summary>Utilities for getting avatar.</summary>
    public static class AvatarUtilities
    {
        /// <summary>Gets URL for retrieving user avatar.</summary>
        /// <param name="userID">ID of the user.</param>
        /// <param name="iconID">Icon ID</param>
        /// <param name="size">Size of the avatar to retrieve.</param>
        /// <remarks><para>This method will return URL to user avatar. You can use this URL with <see cref="HttpClient"/> to download the avatar, or use for any other purpose.</para>
        /// <para><paramref name="size"/> is only a requested size of the avatar. It is not guaranteed that URL will contain avatar with this size.</para>
        /// <para>Currently, changing <paramref name="iconID"/> to an invalid value seems to not cause error when the URL is used. However it is not guaranteed to always be the case, so a valid <see cref="WolfUser.Icon">WolfUser.Icon</see> value is recommended.</para></remarks>
        /// <returns>URL to user's avatar.</returns>
        public static string GetUserAvatarURL(uint userID, int iconID, uint size = 500)
            => $"https://clientavatars.palapi.net/FileServerSpring/subscriber/avatar/{userID}?size={size}&iconId={iconID}";

        /// <summary>Gets URL for retrieving user avatar.</summary>
        /// <param name="user">User to get avatar URL for.</param>
        /// <param name="size">Size of the avatar to retrieve.</param>
        /// <remarks><para>This method will return URL to user avatar. You can use this URL with <see cref="HttpClient"/> to download the avatar, or use for any other purpose.</para>
        /// <para><paramref name="size"/> is only a requested size of the avatar. It is not guaranteed that URL will contain avatar with this size.</para></remarks>
        /// <returns>URL to user's avatar.</returns>
        public static string GetAvatarURL(this WolfUser user, uint size = 500)
            => GetUserAvatarURL(user.ID, user.Icon, size);

        /// <summary>Gets URL for retrieving group avatar.</summary>
        /// <param name="groupID">ID of the group.</param>
        /// <param name="iconID">Icon ID</param>
        /// <param name="size">Size of the avatar to retrieve.</param>
        /// <remarks><para>This method will return URL to group avatar. You can use this URL with <see cref="HttpClient"/> to download the avatar, or use for any other purpose.</para>
        /// <para><paramref name="size"/> is only a requested size of the avatar. It is not guaranteed that URL will contain avatar with this size.</para>
        /// <para>Currently, changing <paramref name="iconID"/> to an invalid value seems to not cause error when the URL is used. However it is not guaranteed to always be the case, so a valid <see cref="WolfGroup.Icon">WolfGroup.Icon</see> value is recommended.</para></remarks>
        /// <returns>URL to group's avatar.</returns>
        public static string GetGroupAvatarURL(uint groupID, int iconID, uint size = 500)
            => $"https://clientavatars.palapi.net/FileServerSpring/group/avatar/{groupID}?size={size}&iconId={iconID}";

        /// <summary>Gets URL for retrieving group avatar.</summary>
        /// <param name="group">Group to get avatar URL for.</param>
        /// <param name="size">Size of the avatar to retrieve.</param>
        /// <remarks><para>This method will return URL to group avatar. You can use this URL with <see cref="HttpClient"/> to download the avatar, or use for any other purpose.</para>
        /// <para><paramref name="size"/> is only a requested size of the avatar. It is not guaranteed that URL will contain avatar with this size.</para></remarks>
        /// <returns>URL to group's avatar.</returns>
        public static string GetAvatarURL(this WolfGroup group, uint size = 500)
            => GetGroupAvatarURL(group.ID, group.Icon, size);
    }
}
