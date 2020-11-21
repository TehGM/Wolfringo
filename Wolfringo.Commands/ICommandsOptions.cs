namespace TehGM.Wolfringo.Commands
{
    /// <summary>Options used for command processing.</summary>
    public interface ICommandsOptions
    {
        /// <summary>Prefix commands need to have.</summary>
        /// <remarks><para>The actual requirement for command to have a prefix is specified by <see cref="RequirePrefix"/>.</para></remarks>
        /// <seealso cref="RequirePrefix"/>
        string Prefix { get; }
        /// <summary>Whether commands should behave case-insensitively by default.</summary>
        /// <remarks><para>This setting can be overwritten per command using <see cref="CaseInsensitiveAttribute"/>.</para></remarks>
        /// <seealso cref="CaseInsensitiveAttribute"/>
        bool CaseInsensitive { get; }
        /// <summary>How prefix requirement is enforced by default.</summary>
        /// <remarks><para>Prefix value can be set using <see cref="Prefix"/></para></remarks>
        /// <seealso cref="Prefix"/>
        PrefixRequirement RequirePrefix { get; }
    }
}
