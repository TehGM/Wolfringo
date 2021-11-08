using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Makes a string parameter contain un-parsed command arguments, without prefix or command name.</summary>
    /// <remarks><para>This is only valid on parameters of type <see cref="string"/>. Will be ignored on other attributes.</para>
    /// <para>With regex command, this will include command name itself, as regex commands have no way to separate command name from arguments.</para></remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ArgumentsTextAttribute : Attribute
    {
        /// <summary>Makes a string parameter contain un-parsed command arguments, without prefix or command name.</summary>
        public ArgumentsTextAttribute() { }
    }
}
