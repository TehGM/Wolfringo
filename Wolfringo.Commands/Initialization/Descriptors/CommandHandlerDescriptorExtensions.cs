namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Extensions for command handler descriptors.</summary>
    public static class CommandHandlerDescriptorExtensions
    {
        /// <summary>Checks if the command handler is persistent.</summary>
        /// <param name="descriptor">Command handler descriptor.</param>
        /// <remarks><para>This value will only return true if handler's type has <see cref="CommandsHandlerAttribute"/>. If the attribute is not present (for example, when handler was force-initialized without command discovery), it'll always return false.</para>
        /// <para>It's up to <see cref="ICommandsHandlerProvider"/> to respect this value. <see cref="CommandsHandlerProvider"/> included with Wolfringo will always respect it and cache handlers if this value is true.</para></remarks>
        /// <returns>True if handler is persistent; otherwise false.</returns>
        public static bool IsPersistent(this ICommandHandlerDescriptor descriptor)
            => descriptor.Attribute != null && descriptor.Attribute.IsPersistent;
    }
}
