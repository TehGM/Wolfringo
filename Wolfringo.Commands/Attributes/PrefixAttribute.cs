using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Overwrites prefix or prefix requirement specified by <see cref="ICommandsOptions"/>.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PrefixAttribute : Attribute
    {
        /// <summary>Prefix for this command. Null means no overwriting.</summary>
        public string PrefixOverride { get; } = null;
        /// <summary>Prefix requirement for this command. Null means no overwriting.</summary>
        public PrefixRequirement? PrefixRequirementOverride { get; } = null;

        /// <param name="prefix">Prefix for this command. Null means no overwriting.</param>
        /// <param name="requirePrefix">Prefix requirement for this command. Null means no overwriting.</param>
        public PrefixAttribute(string prefix, PrefixRequirement? requirePrefix)
        {
            this.PrefixOverride = prefix;
            this.PrefixRequirementOverride = requirePrefix;
        }

        /// <param name="prefix">Prefix for this command. Null means no overwriting.</param>
        public PrefixAttribute(string prefix) : this(prefix, null) { }
        /// <param name="requirePrefix">Prefix requirement for this command.</param>
        public PrefixAttribute(PrefixRequirement requirePrefix) : this(null, requirePrefix) { }
    }
}
