using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing.ArgumentConverters
{
    /// <summary>Argument converter for strings.</summary>
    public class StringConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(string) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => arg;
    }
}
