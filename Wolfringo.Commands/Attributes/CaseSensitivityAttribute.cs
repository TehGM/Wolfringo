using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Sets case sensitivity for the command.</summary>
    /// <remarks><para>Command's case sensitivity will be taken from <see cref="CaseSensitivityAttribute"/> present on the method. If the method doesn't have the attribute, it'll be taken from <see cref="CaseSensitivityAttribute"/> present on the handler type. If the handler type also doesn't specify the priority, value set in <see cref="CommandsOptions"/> will be used.</para>
    /// <para>Respecting case sensitivity depends on the command initializer. All command initializers included with Wolfringo respect command's case sensitivity - respecting both <see cref="CaseSensitivityAttribute"/> and value in <see cref="CommandsOptions"/>.</para></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CaseSensitivityAttribute : Attribute
    {
        /// <summary>Command's case sensitivity.</summary>
        /// <remarks>See <see cref="CaseSensitivityAttribute"/> for more information about command's case sensitivity.</remarks>
        public bool CaseSensitive { get; }

        /// <summary>Creates a new case sensitivity attribute.</summary>
        /// <param name="caseSensitive">Command's case sensitivity.</param>
        /// <remarks>See <see cref="CaseSensitivityAttribute"/> for more information about command's case sensitivity.</remarks>
        public CaseSensitivityAttribute(bool caseSensitive)
        {
            this.CaseSensitive = caseSensitive;
        }
    }
}
