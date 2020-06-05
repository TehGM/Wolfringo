using System.Reflection;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Event callback for received messages and events.</summary>
    /// <remarks>This interface is designed to allow invoking callback conditionally. If <see cref="TryInvoke(IWolfMessage)"/> returns false,
    /// it doesn't meant invoking failed - it means that callback determined it should not invoke for the provided message.</remarks>
    public interface IMessageCallback
    {
        /// <summary>Method to invoke.</summary>
        MethodInfo CallbackInfo { get; }
        /// <summary>Attempts to invoke the callback.</summary>
        /// <param name="message">Message to attempt invoking the callback for.</param>
        /// <returns>True if callback was invoked; otherwise false.</returns>
        bool TryInvoke(IWolfMessage message);
    }
}
