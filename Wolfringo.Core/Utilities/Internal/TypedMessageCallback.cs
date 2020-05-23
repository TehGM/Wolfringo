using System;
using System.Reflection;

namespace TehGM.Wolfringo.Utilities.Internal
{
    public class TypedMessageCallback<T> : IMessageCallback, IEquatable<TypedMessageCallback<T>> where T : IWolfMessage
    {
        public MethodInfo CallbackInfo => _callback.Method;
        private readonly Action<T> _callback;

        public TypedMessageCallback(Action<T> callback)
        {
            this._callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public virtual bool TryInvoke(IWolfMessage message)
        {
            if (message is T msg)
            {
                _callback.Invoke(msg);
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
            => Equals(obj as TypedMessageCallback<T>);

        public bool Equals(TypedMessageCallback<T> other)
            => other != null && CallbackInfo.Equals(other.CallbackInfo);

        public override int GetHashCode()
            => -1397514458 + CallbackInfo.GetHashCode();
    }
}
