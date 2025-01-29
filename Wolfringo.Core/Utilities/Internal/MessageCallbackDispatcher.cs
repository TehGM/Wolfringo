using System.Collections.Generic;
using System.Threading;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Utility holding and invoking message callbacks.</summary>
    public class MessageCallbackDispatcher
    {
        private readonly List<IMessageCallback> _callbacks = new List<IMessageCallback>();
#if NET9_0_OR_GREATER
        private readonly Lock _lock = new Lock();
#else
        private readonly object _lock = new object();
#endif

        /// <summary>Adds message callback.</summary>
        /// <param name="callback">Callback to add.</param>
        public void Add(IMessageCallback callback)
        {
            lock (this._lock)
                this._callbacks.Add(callback);
        }

        /// <summary>Remove message callback.</summary>
        /// <param name="callback">Callback to remove.</param>
        public void Remove(IMessageCallback callback)
        {
            lock (this._lock)
            {
                for (int i = this._callbacks.Count - 1; i >= 0; i--)
                {
                    if (this._callbacks[i].Equals(callback))
                        this._callbacks.RemoveAt(i);
                }
            }
        }

        /// <summary>Attempts to invoke all callbacks.</summary>
        /// <param name="message">Message to pass to callbacks.</param>
        public void Invoke(IWolfMessage message)
        {
            lock (this._lock)
            {
                for (int i = 0; i < this._callbacks.Count; i++)
                    this._callbacks[i].TryInvoke(message);
            }
        }
    }
}
