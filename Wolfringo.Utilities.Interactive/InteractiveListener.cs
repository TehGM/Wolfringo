using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Listener allowing to await next message that matches specified conditions.</summary>
    /// <typeparam name="T">Type of message</typeparam>
    public class InteractiveListener<T> : IInteractiveListener<T> where T : IWolfMessage
    {
        private readonly Func<T, bool> _conditions;

        /// <summary>Creates a new interactive listener.</summary>
        /// <param name="conditions">Conditions that received message needs to match.</param>
        public InteractiveListener(Func<T, bool> conditions)
        {
            this._conditions = conditions;
        }

        /// <inheritdoc/>
        public async Task<T> AwaitNextAsync(IWolfClient client, CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            using (cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken)))
            {
                Action<T> callback = null;
                callback = message =>
                {
                    if (!this._conditions(message))
                        return;
                    client.RemoveMessageListener<T>(callback);
                    tcs.TrySetResult(message);
                };

                client.AddMessageListener(callback);
                return await tcs.Task;
            }
        }
    }
}
