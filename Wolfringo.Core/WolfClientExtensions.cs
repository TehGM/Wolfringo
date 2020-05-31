using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    public static class WolfClientExtensions
    {
        public static Task<WolfResponse> SendAsync(this IWolfClient client, IWolfMessage message, CancellationToken cancellationToken = default)
            => client.SendAsync<WolfResponse>(message, cancellationToken);

        public static void AddMessageListener<T>(this IWolfClient client, Action<T> listener) where T : IWolfMessage
            => client.AddMessageListener(new TypedMessageCallback<T>(listener));
        public static void AddMessageListener<T>(this IWolfClient client, string command, Action<T> listener) where T : IWolfMessage
            => client.AddMessageListener(new CommandMessageCallback<T>(command, listener));
        public static void RemoveMessageListener<T>(this IWolfClient client, Action<T> listener) where T : IWolfMessage
            => client.RemoveMessageListener(new TypedMessageCallback<T>(listener));
    }
}
