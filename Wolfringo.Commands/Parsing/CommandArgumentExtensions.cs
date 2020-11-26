using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing
{
    public static class CommandArgumentExtensions
    {
        /// <summary>Loads [ConvertingError] attribute specified for a param, or default if not present.</summary>
        /// <returns>Instance of attribute.</returns>
        public static ConvertingErrorAttribute GetConvertingErrorAttribute(this ParameterInfo parameter)
            => parameter.GetCustomAttribute<ConvertingErrorAttribute>(true) ?? ConvertingErrorAttribute.Default;

        /// <summary>Loads [MissingError] attribute specified for a param, or default if not present.</summary>
        /// <returns>Instance of attribute.</returns>
        public static MissingErrorAttribute GetMissingErrorAttribute(this ParameterInfo parameter)
            => parameter.GetCustomAttribute<MissingErrorAttribute>(true) ?? MissingErrorAttribute.Default;
    }
}
