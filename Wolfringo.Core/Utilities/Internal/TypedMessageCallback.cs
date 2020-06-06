using System;
using System.Reflection;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <inheritdoc/>
    /// <remarks><para>This interface is designed to allow invoking callback conditionally. If <see cref="TryInvoke(IWolfMessage)"/> returns false,
    /// it doesn't meant invoking failed - it means that callback determined it should not invoke for the provided message.</para>
    /// <para>This callback will only invoke if message is of type <typeparamref name="T"/>.</para></remarks>
    public class TypedMessageCallback<T> : IMessageCallback, IEquatable<TypedMessageCallback<T>> where T : IWolfMessage
    {
        /// <inheritdoc/>
        public MethodInfo CallbackInfo => _callback.Method;
        private readonly Action<T> _callback;

        /// <summary>Creates callback instance.</summary>
        /// <param name="callback">Method to invoke when this callback invokes.</param>
        public TypedMessageCallback(Action<T> callback)
        {
            this._callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <inheritdoc/>
        /// <remarks>Callback will only invoke if <paramref name="message"/> is of type <typeparamref name="T"/>.</remarks>
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
