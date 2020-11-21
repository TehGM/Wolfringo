using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Sets case insensitivity for the command.</summary>
    /// <remarks><para>Command's case insensitivity will be taken from <see cref="CaseInsensitiveAttribute"/> present on the method. If the method doesn't have the attribute, it'll be taken from <see cref="CaseInsensitiveAttribute"/> present on the handler type. If the handler type also doesn't specify the priority, value set in <see cref="ICommandsOptions"/> will be used.</para>
    /// <para>Respecting case insensitivity depends on the command initializer. All command initializers included with Wolfringo respect command's case insensitivy - respecting both <see cref="CaseInsensitiveAttribute"/> and value in <see cref="ICommandsOptions"/>.</para></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CaseInsensitiveAttribute : Attribute
    {
        /// <summary>Command's case insensitivity.</summary>
        /// <remarks>See <see cref="CaseInsensitiveAttribute"/> for more information about command's case insensitivity.</remarks>
        public bool CaseInsensitive { get; }

        /// <summary>Creates a new case insensitive attribute.</summary>
        /// <param name="caseInsensitive">Command's case insensitivity.</param>
        /// <remarks>See <see cref="CaseInsensitiveAttribute"/> for more information about command's case insensitivity.</remarks>
        public CaseInsensitiveAttribute(bool caseInsensitive)
        {
            this.CaseInsensitive = caseInsensitive;
        }
    }
}
