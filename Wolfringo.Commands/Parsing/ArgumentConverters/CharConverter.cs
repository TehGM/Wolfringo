using System;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing.ArgumentConverters
{
    /// <summary>Argument converter for boolean.</summary>
    public class CharConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Char) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
        {
            if (arg.Length != 1)
                throw new InvalidOperationException($"Cannot convert {arg} to {typeof(Char).Name}");
            return arg[0];
        }
    }
}
