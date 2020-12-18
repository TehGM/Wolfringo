using System;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <inheritdoc/>
    /// <remarks><para>This interface is designed to allow invoking callback conditionally. If <see cref="TryInvoke(IWolfMessage)"/> returns false,
    /// it doesn't meant invoking failed - it means that callback determined it should not invoke for the provided message.</para>
    /// <para>This callback will only invoke if message is of type <typeparamref name="T"/>, 
    /// and message's <see cref="IWolfMessage.EventName"/> is the same as the one provided in class constructor.</para></remarks>
    public class CommandMessageCallback<T> : TypedMessageCallback<T> where T : IWolfMessage
    {
        /// <summary>Callback will be invoked for messages with command matching this value.</summary>
        public string Command { get; }

        /// <summary>Creates callback instance.</summary>
        /// <param name="command">Command that must be matched for message to execute the callback.</param>
        /// <param name="callback">Method to invoke when this callback invokes.</param>
        public CommandMessageCallback(string command, Action<T> callback) : base(callback)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            this.Command = command;
        }

        /// <inheritdoc/>
        /// <remarks>This callback will only invoke if <paramref name="message"/> is of type <typeparamref name="T"/>, 
        /// and <paramref name="message"/>'s <see cref="IWolfMessage.EventName"/> is the same as the one provided in class constructor.</remarks>
        public override bool TryInvoke(IWolfMessage message)
        {
            if (!string.Equals(message.EventName, this.Command, StringComparison.OrdinalIgnoreCase))
                return false;
            return base.TryInvoke(message);
        }
    }
}
