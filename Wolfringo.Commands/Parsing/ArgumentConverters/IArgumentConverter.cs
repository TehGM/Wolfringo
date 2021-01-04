using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>A converter for a command method argument that converts string value to a specific parameter type.</summary>
    public interface IArgumentConverter
    {
        /// <summary>Determines whether this converter can convert an argument to given parameter.</summary>
        /// <param name="parameter">Parameter to convert the argument to.</param>
        /// <returns>True if this converter can be used for the conversion; otherwise false.</returns>
        bool CanConvert(ParameterInfo parameter);
        /// <summary>Converts an argument to given parameter.</summary>
        /// <param name="parameter">Parameter to convert the argument to.</param>
        /// <param name="arg">Argument to convert.</param>
        /// <returns>Argument converted to given type.</returns>
        object Convert(ParameterInfo parameter, string arg);
    }
}
