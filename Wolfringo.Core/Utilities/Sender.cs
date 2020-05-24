using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Types;

namespace TehGM.Wolfringo
{
    public static class Sender
    {
        #region Logging in and out
        public static async Task<LoginResponse> LoginAsync(this IWolfClient client, string login, string password, bool isPasswordAlreadyHashed = false, CancellationToken cancellationToken = default)
        {
            LoginResponse response = await client.SendAsync<LoginResponse>(new LoginMessage(login, password, isPasswordAlreadyHashed), cancellationToken).ConfigureAwait(false);
            await client.SendAsync(new SubscribeToPmMessage(), cancellationToken).ConfigureAwait(false);
            await client.SendAsync(new SubscribeToGroupMessage(), cancellationToken).ConfigureAwait(false);
            return response;
        }
        #endregion

        #region Notifications
        public static async Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, WolfLanguage language, WolfDevice device, CancellationToken cancellationToken = default)
        {
            ListNotificationsResponse response = await client.SendAsync<ListNotificationsResponse>(new ListNotificationsMessage(language, device), cancellationToken).ConfigureAwait(false);
            return response.Notifications?.Any() == true ? response.Notifications : Enumerable.Empty<WolfNotification>();
        }
        public static Task<IEnumerable<WolfNotification>> GetNotificationsAsync(this IWolfClient client, CancellationToken cancellationToken = default)
            => client.GetNotificationsAsync(WolfLanguage.English, WolfDevice.Bot, cancellationToken);
        #endregion
    }
}
