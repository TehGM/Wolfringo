using System;
using System.Collections.Generic;
using System.Reflection;

namespace TehGM.Wolfringo.Commands
{
    /// <inheritdoc/>
    public class CommandsOptions : ICommandsOptions
    {
        /// <inheritdoc/>
        public string Prefix { get; set; } = "!";
        /// <inheritdoc/>
        public bool CaseSensitivity { get; set; } = false;
        /// <inheritdoc/>
        public PrefixRequirement RequirePrefix { get; set; } = PrefixRequirement.Always;

        // for loading
        /// <summary>Collection of Types to load as Command Handlers.</summary>
        /// <remarks>Any type included in this collection does not need to have <see cref="CommandHandlerAttribute"/>.</remarks>
        /// <seealso cref="Assemblies"/>
        public ICollection<Type> Classes { get; set; } = new List<Type>();
        /// <summary>Collection of Assemblies to load Command Handlers from.</summary>
        /// <remarks>Types need to have <see cref="CommandHandlerAttribute"/> to be treated as a loadable type. Any type without that attribute will be ignored.</remarks>
        public ICollection<Assembly> Assemblies { get; set; } = new List<Assembly>() { Assembly.GetEntryAssembly() };
    }
}
