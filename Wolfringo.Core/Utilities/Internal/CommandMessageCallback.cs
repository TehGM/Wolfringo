using System;

namespace TehGM.Wolfringo.Utilities.Internal
{
    public class CommandMessageCallback<T> : TypedMessageCallback<T> where T : IWolfMessage
    {
        public string Command { get; }

        public CommandMessageCallback(string command, Action<T> callback) : base(callback)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            this.Command = command;
        }

        public override bool TryInvoke(IWolfMessage message)
        {
            if (!string.Equals(message.Command, this.Command, StringComparison.OrdinalIgnoreCase))
                return false;
            return base.TryInvoke(message);
        }
    }
}
