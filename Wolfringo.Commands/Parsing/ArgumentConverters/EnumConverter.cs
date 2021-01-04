using System;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing.ArgumentConverters
{
    /// <summary>Argument converter for any type of enum.</summary>
    public class EnumConverter : IArgumentConverter
    {
        /// <summary>Whether case should be ignored when parsing enum value.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool IgnoreCase { get; set; } = true;

        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => parameter.ParameterType.IsEnum;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => Enum.Parse(parameter.ParameterType, arg, this.IgnoreCase);
    }
}
