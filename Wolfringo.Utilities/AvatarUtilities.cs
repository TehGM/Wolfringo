using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

        /// <summary>Downloads bytes of user avatar.</summary>
        /// <param name="user">User to download avatar of.</param>
        /// <param name="size">Size of the avatar to request.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <remarks>This method creates a temporary HttpClient just for the request. This might be performance heavy, and therefore it's recommended to provide your own client from own cache, or using a client factory.</remarks>
        /// <returns>Bytes of user avatar. Null if user of avatar not found.</returns>
        public static async Task<byte[]> DownloadAvatarAsync(this WolfUser user, uint size = 500, CancellationToken cancellationToken = default)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", GetDefaultUserAgent());
                return await DownloadAvatarAsync(user, client, size, cancellationToken).ConfigureAwait(false);
            }
        }
        /// <summary>Downloads bytes of user avatar.</summary>
        /// <param name="user">User to download avatar of.</param>
        /// <param name="client">Http Client to request avatar bytes with.</param>
        /// <param name="size">Size of the avatar to request.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Bytes of user avatar. Null if user of avatar not found.</returns>
        public static Task<byte[]> DownloadAvatarAsync(this WolfUser user, HttpClient client, uint size = 500, CancellationToken cancellationToken = default)
            => DownloadAvatarAsync(GetAvatarURL(user, size), client, cancellationToken);

        /// <summary>Downloads bytes of group avatar.</summary>
        /// <param name="group">Group to download avatar of.</param>
        /// <param name="size">Size of the avatar to request.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <remarks>This method creates a temporary HttpClient just for the request. This might be performance heavy, and therefore it's recommended to provide your own client from own cache, or using a client factory.</remarks>
        /// <returns>Bytes of group avatar. Null if group of avatar not found.</returns>
        public static async Task<byte[]> DownloadAvatarAsync(this WolfGroup group, uint size = 500, CancellationToken cancellationToken = default)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", GetDefaultUserAgent());
                return await DownloadAvatarAsync(group, client, size, cancellationToken).ConfigureAwait(false);
            }
        }
        /// <summary>Downloads bytes of group avatar.</summary>
        /// <param name="group">Group to download avatar of.</param>
        /// <param name="client">Http Client to request avatar bytes with.</param>
        /// <param name="size">Size of the avatar to request.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Bytes of group avatar. Null if group of avatar not found.</returns>
        public static Task<byte[]> DownloadAvatarAsync(this WolfGroup group, HttpClient client, uint size = 500, CancellationToken cancellationToken = default)
            => DownloadAvatarAsync(GetAvatarURL(group, size), client, cancellationToken);

        private static string GetDefaultUserAgent()
        {
            AssemblyName asmName = Assembly.GetEntryAssembly().GetName();
            return $"{asmName.Name} {asmName.Version} (Powered by Wolfringo - https://wolfringo.tehgm.net)";
        }

        /// <summary>Downloads avatar bytes.</summary>
        /// <param name="url">URL of the avatar.</param>
        /// <param name="client">Client to download with.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Avatar bytes. Null if entity or avatar was not found.</returns>
        private static async Task<byte[]> DownloadAvatarAsync(string url, HttpClient client, CancellationToken cancellationToken = default)
        {
            using (HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    try
                    {
                        string responseRaw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        JObject responseJson = JObject.Parse(responseRaw);
                        if (responseJson["code"].Value<int>() == 8)
                            return null;
                    }
                    catch { }
                }
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
        }
    }
}
