using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Marks a type as a command handler.</summary>
    /// <remarks><para>By default, Commands System will only load comands from types marked with <see cref="CommandHandlerAttribute"/>, unless they're explicitly added to <see cref="CommandsOptions"/>'s <see cref="CommandsOptions.Classes"/> collection.</para>
    /// <para>It is recommended to mark handlers with this attribute regardless of loading method.</para>
    /// <para>Default Wolfringo commands loading logic will ignore static classes and structs.</para></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class CommandHandlerAttribute : Attribute
    {
        /// <summary>Whether the command handler is persistent.</summary>
        /// <remarks><para>Persistent handlers will be initialized once, and kept in Commands System's memory during its lifetime. This will disable the default behaviour of reinitializing handler each time a command is executed. This is especially useful when the handler needs to keep any state, or listens to <see cref="IWolfClient"/>'s events.</para>
        /// <para>It's up to <see cref="Initialization.ICommandHandlerProvider"/> to respect this value. <see cref="Initialization.CommandHandlerProvider"/> included with Wolfringo will always respect it and cache handlers if this value is true.</para></remarks>
        public bool IsPersistent { get; set; } = false;
        /// <summary>Whether the command handler is is pre-initialized.</summary>
        /// <remarks><para>By default, command handlers are only created just before they're executed for the very first time. Setting this value to true will cause handler to be initialized as soon as the command is loaded. This is especially useful if the handler listens to <see cref="IWolfClient"/>'s events, or is expensive to instantiate.</para>
        /// <para>This setting is meant to be used together with <see cref="IsPersistent"/>, however this is not required.</para>
        /// <para>It's up to <see cref="CommandsService"/> to respect this value. <see cref="CommandsService"/> included with Wolfringo will always respect it and trigger creation of the handler if this value is true.</para></remarks>
        public bool PreInitialize { get; set; } = false;
    }
}
