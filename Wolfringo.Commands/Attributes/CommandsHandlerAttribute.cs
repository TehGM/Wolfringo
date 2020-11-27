using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Marks a type as a command handler.</summary>
    /// <remarks><para>By default, Commands System will only load comands from types marked with <see cref="CommandsHandlerAttribute"/>, unless they're explicitly added to <see cref="CommandsOptions"/>'s <see cref="CommandsOptions.Classes"/> collection.</para>
    /// <para>It is recommended to mark handlers with this attribute regardless of loading method.</para>
    /// <para>Default Wolfringo commands loading logic will ignore static classes and structs.</para></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class CommandsHandlerAttribute : Attribute
    {
        /// <summary>Whether the command handler is persistent.</summary>
        /// <remarks><para>Persistent handlers will be initialized once, and kept in Commands System's memory during its lifetime. This will disable the default behaviour of reinitializing handler each time a command is executed. This is especially useful when the handler needs to keep any state, or listens to <see cref="IWolfClient"/>'s events.</para>
        /// <para>It's up to <see cref="Initialization.ICommandsHandlerProvider"/> to respect this value. <see cref="Initialization.CommandsHandlerProvider"/> included with Wolfringo will always respect it and cache handlers if this value is true.</para></remarks>
        public bool IsPersistent { get; set; } = false;
    }
}
