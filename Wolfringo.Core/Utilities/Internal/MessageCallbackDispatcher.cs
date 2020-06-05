using System.Collections.Generic;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Utility holding and invoking message callbacks.</summary>
    public class MessageCallbackDispatcher
    {
        private readonly List<IMessageCallback> _callbacks = new List<IMessageCallback>();

        /// <summary>Adds message callback.</summary>
        /// <param name="callback">Callback to add.</param>
        public void Add(IMessageCallback callback)
        {
            lock (_callbacks)
                _callbacks.Add(callback);
        }

        /// <summary>Remove message callback.</summary>
        /// <param name="callback">Callback to remove.</param>
        public void Remove(IMessageCallback callback)
        {
            lock (_callbacks)
            {
                for (int i = _callbacks.Count - 1; i >= 0; i--)
                {
                    if (_callbacks[i].Equals(callback))
                        _callbacks.RemoveAt(i);
                }
            }
        }

        /// <summary>Attempts to invoke all callbacks.</summary>
        /// <param name="message">Message to pass to callbacks.</param>
        public void Invoke(IWolfMessage message)
        {
            lock (_callbacks)
            {
                for (int i = 0; i < _callbacks.Count; i++)
                    _callbacks[i].TryInvoke(message);
            }
        }
    }
}
