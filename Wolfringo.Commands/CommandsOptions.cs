using System;
using System.Collections.Generic;
using System.Reflection;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Options used for command processing.</summary>
    /// <remarks><para>These are default options for <see cref="CommandsService"/>. They contain minimum amount of settings for core of Commands System to work.</para>
    /// <para>If you need to create custom options, inherit from this class. All properties are settable, so they can be changed from child classes.</para></remarks>
    public class CommandsOptions : ICommandOptions
    {
        /// <inheritdoc/>
        /// <summary>Prefix commands need to have. Default value is "!".</summary>
        public string Prefix { get; set; } = "!";
        /// <inheritdoc/>
        /// <summary>Whether commands should behave case-sensitive by default. Default value is false.</summary>
        public bool CaseSensitivity { get; set; } = false;
        /// <inheritdoc/>
        /// <summary>How prefix requirement is enforced by default. Default value is "Always".</summary>
        /// <seealso cref="Prefix"/>
        public PrefixRequirement RequirePrefix { get; set; } = PrefixRequirement.Always;
        /// <summary>Whether the built-in default help command should be enabled.</summary>
        /// <remarks><para>This command will be added independently on <see cref="Classes"/> and <see cref="Assemblies"/>.</para>
        /// <para>Defaults to false.</para></remarks>
        public bool EnableDefaultHelpCommand { get; set; } = false;

        // for loading
        /// <summary>Collection of Types to load as Command Handlers.</summary>
        /// <remarks>Any type included in this collection does not need to have <see cref="CommandsHandlerAttribute"/>.</remarks>
        /// <seealso cref="Assemblies"/>
        public ICollection<Type> Classes { get; set; } = new HashSet<Type>();
        /// <summary>Collection of Assemblies to load Command Handlers from.</summary>
        /// <remarks>Types need to have <see cref="CommandsHandlerAttribute"/> to be treated as a loadable type. Any type without that attribute will be ignored.</remarks>
        public ICollection<Assembly> Assemblies { get; set; } = new HashSet<Assembly>() { Assembly.GetEntryAssembly() };
    }
}
