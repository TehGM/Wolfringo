using System;
using System.Collections.Generic;
using System.Reflection;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Options used for command processing.</summary>
    /// <remarks><para>These are default options for <see cref="CommandsService"/>. They contain minimum amount of settings for core of Commands System to work.</para>
    /// <para>If you need to create custom options, inherit from this class. All properties are settable, so they can be changed from child classes.</para></remarks>
    public class CommandsOptions
    {
        /// <summary>Prefix commands need to have.</summary>
        /// <remarks><para>The actual requirement for command to have a prefix is specified by <see cref="RequirePrefix"/>.</para></remarks>
        /// <seealso cref="RequirePrefix"/>
        public string Prefix { get; set; } = "!";
        /// <summary>Whether commands should behave case-sensitive by default.</summary>
        /// <remarks><para>This setting can be overwritten per command using <see cref="CaseSensitivityAttribute"/>.</para></remarks>
        /// <seealso cref="CaseSensitivityAttribute"/>
        public bool CaseSensitivity { get; set; } = false;
        /// <summary>How prefix requirement is enforced by default.</summary>
        /// <remarks><para>Prefix value can be set using <see cref="Prefix"/></para></remarks>
        /// <seealso cref="Prefix"/>
        public PrefixRequirement RequirePrefix { get; set; } = PrefixRequirement.Always;

        // for loading
        /// <summary>Collection of Types to load as Command Handlers.</summary>
        /// <remarks>Any type included in this collection does not need to have <see cref="CommandHandlerAttribute"/>.</remarks>
        /// <seealso cref="Assemblies"/>
        public ICollection<Type> Classes { get; set; } = new HashSet<Type>();
        /// <summary>Collection of Assemblies to load Command Handlers from.</summary>
        /// <remarks>Types need to have <see cref="CommandHandlerAttribute"/> to be treated as a loadable type. Any type without that attribute will be ignored.</remarks>
        public ICollection<Assembly> Assemblies { get; set; } = new HashSet<Assembly>() { Assembly.GetEntryAssembly() };
    }
}
