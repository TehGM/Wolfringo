using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>Provides a command argument converter.</summary>
    public interface IArgumentConverterProvider
    {
        /// <summary>Gets a converter for the parameter.</summary>
        /// <param name="parameter">Parameter to convert.</param>
        /// <returns>A converter for provided parameter.</returns>
        IArgumentConverter GetConverter(ParameterInfo parameter);
    }
}
