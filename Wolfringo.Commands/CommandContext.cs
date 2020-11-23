using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Commands
{
    /// <inheritdoc/>
    public class CommandContext : ICommandContext
    {
        /// <inheritdoc/>
        IChatMessage ICommandContext.Message => this.Message;
        /// <summary>Chat message that triggered the command.</summary>
        public ChatMessage Message { get; }
        /// <inheritdoc/>
        public IWolfClient Client { get; }
        /// <inheritdoc/>
        public ICommandsOptions Options { get; }

        /// <summary>Whether the message was sent in a group.</summary>
        public bool IsGroup => this.Message.IsGroupMessage;
        /// <summary>Whether the message was sent in PM.</summary>
        public bool IsPrivate => this.Message.IsPrivateMessage;

        /// <summary>Creates a command context.</summary>
        /// <param name="message">Chat message that triggered the command.</param>
        /// <param name="client">WOLF client that received the message.</param>
        /// <param name="options">Default options to use for processing the command.</param>
        public CommandContext(ChatMessage message, IWolfClient client, ICommandsOptions options)
        {
            this.Message = message;
            this.Client = client;
            this.Options = options;
        }
    }
}
