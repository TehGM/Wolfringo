using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Overwrites prefix requirement specified by <see cref="CommandsOptions"/>.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PrefixRequirementAttribute : Attribute
    {
        /// <summary>Prefix requirement for this command. Null means no overwriting.</summary>
        public PrefixRequirement? PrefixRequirementOverride { get; } = null;

        /// <param name="requirePrefix">Prefix requirement for this command.</param>
        public PrefixRequirementAttribute(PrefixRequirement requirePrefix)
        {
            this.PrefixRequirementOverride = requirePrefix;
        }
    }
}
