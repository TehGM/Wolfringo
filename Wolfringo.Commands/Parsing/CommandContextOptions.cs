using TehGM.Wolfringo.Commands.Initialization;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>Computed options for a single execution of command.</summary>
    public class CommandContextOptions : ICommandOptions
    {
        /// <inheritdoc/>
        /// <summary>Prefix of the executing command.</summary>
        public string Prefix { get; }
        /// <inheritdoc/>
        /// <summary>Prefix requirement of the executing command.</summary>
        public PrefixRequirement RequirePrefix { get; }
        /// <inheritdoc/>
        /// <summary>Case sensitivity of the executing command.</summary>
        public bool CaseSensitivity { get; }

        /// <summary>Creates a new options instance for command execution context.</summary>
        /// <param name="prefix">Prefix of the executing command.</param>
        /// <param name="requirePrefix">Prefix requirement of the executing command.</param>
        /// <param name="caseSensitivity">Case sensitivity of the executing command.</param>
        public CommandContextOptions(string prefix, PrefixRequirement requirePrefix, bool caseSensitivity)
        {
            this.Prefix = prefix;
            this.RequirePrefix = requirePrefix;
            this.CaseSensitivity = caseSensitivity;
        }

        /// <summary>Builds options for command exection context from base options and descriptor with overrides.</summary>
        /// <param name="originalOptions">Base options.</param>
        /// <param name="descriptor">Command descriptor that overrides options.</param>
        /// <returns>A new options instance for command execution context.</returns>
        public static CommandContextOptions Build(ICommandOptions originalOptions, ICommandInstanceDescriptor descriptor)
        {
            return new CommandContextOptions(
                prefix: descriptor.GetPrefixOverride() ?? originalOptions.Prefix,
                requirePrefix: descriptor.GetPrefixRequirementOverride() ?? originalOptions.RequirePrefix,
                caseSensitivity: descriptor.GetCaseSensitivityOverride() ?? originalOptions.CaseSensitivity);
        }
    }
}
