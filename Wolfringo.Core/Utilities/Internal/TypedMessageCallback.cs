using System;
using System.Collections.Generic;
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

        public bool TryInvoke(IWolfMessage message)
        {
            if (message is T msg)
            {
                _callback.Invoke(msg);
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TypedMessageCallback<T>);
        }

        public bool Equals(TypedMessageCallback<T> other)
        {
            return other != null &&
                   EqualityComparer<MethodInfo>.Default.Equals(CallbackInfo, other.CallbackInfo);
        }

        public override int GetHashCode()
        {
            return -1397514458 + CallbackInfo.GetHashCode();
        }
    }
}
