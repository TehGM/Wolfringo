using System;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public static class CommandInstanceDescriptorExtensions
    {
        public static CommandHandlerAttribute GetHandlerAttribute(this ICommandInstanceDescriptor descriptor)
            => GetHandlerType(descriptor).GetCustomAttribute<CommandHandlerAttribute>(true);

        public static int GetPriority(this ICommandInstanceDescriptor descriptor)
            // on-method priority overwrites handler priority. Default is 0.
            =>  descriptor.Method.GetCustomAttribute<PriorityAttribute>()?.Priority ??
                GetHandlerType(descriptor).GetCustomAttribute<PriorityAttribute>()?.Priority ??
                0;

        public static Type GetHandlerType(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.DeclaringType;
    }
}
