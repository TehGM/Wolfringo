using TehGM.Wolfringo.Commands.Parsing;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Represents options for a command. Base abstraction for <see cref="CommandsOptions"/> and <see cref="CommandContextOptions"/></summary>
    public interface ICommandOptions
    {
        /// <summary>Prefix commands need to have.</summary>
        /// <remarks><para>The actual requirement for command to have a prefix is specified by <see cref="RequirePrefix"/>.</para></remarks>
        /// <seealso cref="RequirePrefix"/>
        string Prefix { get; }
        /// <summary>How prefix requirement is enforced by default.</summary>
        /// <remarks><para>Prefix value can be set using <see cref="Prefix"/></para></remarks>
        /// <seealso cref="Prefix"/>
        PrefixRequirement RequirePrefix { get; }
        /// <summary>Whether commands should behave case-sensitive by default.</summary>
        /// <remarks><para>This setting can be overwritten per command using <see cref="CaseSensitivityAttribute"/>.</para></remarks>
        /// <seealso cref="CaseSensitivityAttribute"/>
        bool CaseSensitivity { get; }
    }
}
