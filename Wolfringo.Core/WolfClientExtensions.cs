using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    public static class WolfClientExtensions
    {
        /// <summary>Sends message, and waits for response from the server.</summary>
        /// <remarks><para>If client uses <see cref="ResponseTypeResolver"/>, the type of response provided with 
        /// <see cref="ResponseTypeAttribute"/> on <paramref name="message"/> will be used for deserialization.<br/>
        /// Otherwise, response will be deserialized as<see cref="WolfResponse"/>.</para></remarks>
        /// <param name="message">Message to send.</param>
        /// <returns>Sending response.</returns>
        /// <exception cref="MessageSendingException">Server responded with error.</exception>
        public static Task<WolfResponse> SendAsync(this IWolfClient client, IWolfMessage message, CancellationToken cancellationToken = default)
            => client.SendAsync<WolfResponse>(message, cancellationToken);

        /// <summary>Adds event listener, invoking when received message is of correct type.</summary>
        /// <typeparam name="T">Type of received message to invoke callback for.</typeparam>
        /// <param name="callback">Callback to invoke on event.</param>
        public static void AddMessageListener<T>(this IWolfClient client, Action<T> callback) where T : IWolfMessage
            => client.AddMessageListener(new TypedMessageCallback<T>(callback));
        /// <summary>Adds event listener, invoking when received message is of correct type and has matching command.</summary>
        /// <typeparam name="T">Type of received message to invoke callback for.</typeparam>
        /// <param name="command">Message command that has to match for callback to be invoked.</param>
        /// <param name="callback">Callback to invoke on event.</param>
        public static void AddMessageListener<T>(this IWolfClient client, string command, Action<T> callback) where T : IWolfMessage
            => client.AddMessageListener(new CommandMessageCallback<T>(command, callback));
        /// <summary>Removes event listener.</summary>
        /// <remarks>Provided type <typeparamref name="T"/> must be the same as the type used when adding the listener.</remarks>
        /// <param name="callback">Callback to remove.</param>
        public static void RemoveMessageListener<T>(this IWolfClient client, Action<T> callback) where T : IWolfMessage
            => client.RemoveMessageListener(new TypedMessageCallback<T>(callback));
    }
}
