using System;
using System.Globalization;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing.ArgumentConverters
{
    /// <summary>Argument converter for boolean.</summary>
    public class BooleanConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Boolean) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToBoolean(arg, CultureInfo.InvariantCulture);
    }
}
