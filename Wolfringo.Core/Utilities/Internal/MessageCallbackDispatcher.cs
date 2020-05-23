using System.Collections.Generic;

namespace TehGM.Wolfringo.Utilities.Internal
{
    public class MessageCallbackDispatcher
    {
        private readonly List<IMessageCallback> _callbacks = new List<IMessageCallback>();

        public void Add(IMessageCallback callback)
        {
            lock (_callbacks)
                _callbacks.Add(callback);
        }

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
