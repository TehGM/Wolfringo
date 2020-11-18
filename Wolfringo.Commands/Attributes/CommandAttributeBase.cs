using System;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class CommandAttributeBase : Attribute
    {
        /// <summary>Overwrite <see cref="CommandsOptions.CaseInsensitive"/> value.</summary>
        /// <remarks><para>Setting this to true or false will make Command Runners ignore <see cref="CommandsOptions.CaseInsensitive"/> option and use the override value provided; leaving as null will not override <see cref="CommandsOptions.CaseInsensitive"/>.</para>
        /// <para>It's up to Command Runner to honour this value. All built-in Wolfringo Command Runners will honour it. It is advised for custom runners to honour this value as well, but this cannot be guaranteed.</para></remarks>
        public bool? OverrideCaseInsensitive { get; set; } = null;
    }
}
