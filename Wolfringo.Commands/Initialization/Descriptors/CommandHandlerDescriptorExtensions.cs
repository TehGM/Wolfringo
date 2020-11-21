namespace TehGM.Wolfringo.Commands.Initialization
{
    public static class CommandHandlerDescriptorExtensions
    {
        public static bool IsPersistent(this ICommandHandlerDescriptor descriptor)
            => descriptor.Attribute != null && descriptor.Attribute.IsPersistent;

        public static bool IsPreInitialized(this ICommandHandlerDescriptor descriptor)
            => descriptor.Attribute != null && descriptor.Attribute.PreInitialize;
    }
}
