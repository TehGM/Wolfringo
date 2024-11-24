using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Overwrites prefix specified by <see cref="CommandsOptions"/>.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PrefixAttribute : Attribute
    {
        /// <summary>Prefix for this command. Null means no overwriting.</summary>
        public string PrefixOverride { get; } = null;

        /// <param name="prefix">Prefix for this command. Null means no overwriting.</param>
        public PrefixAttribute(string prefix)
        {
            this.PrefixOverride = prefix;
        }
    }
}
