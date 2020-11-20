using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public static class CommandInstanceDescriptorExtensions
    {
        public static CommandHandlerAttribute GetHandlerAttribute(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.DeclaringType.GetCustomAttribute<CommandHandlerAttribute>(true);

        public static int GetPriority(this ICommandInstanceDescriptor descriptor)
            // on-method priority overwrites handler priority. Default is 0.
            =>  descriptor.Method.GetCustomAttribute<PriorityAttribute>()?.Priority ??
                descriptor.Method.DeclaringType.GetCustomAttribute<PriorityAttribute>()?.Priority ??
                0;
    }
}
