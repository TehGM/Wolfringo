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

        /// <summary>Gets name of the argument.</summary>
        /// <remarks>If <see cref="ArgumentNameAttribute"/> is specified for the parameter, its value will be used. If it's missing, it will default to parameter name.</remarks>
        /// <returns>Argument name.</returns>
        public static string GetArgumentName(this ParameterInfo parameter)
            => parameter.GetCustomAttribute<ArgumentNameAttribute>()?.Name ?? parameter.Name;

        /// <summary>Gets name of the argument's type.</summary>
        /// <remarks>If <see cref="ArgumentTypeNameAttribute"/> is specified for the parameter, its value will be used. If it's missing, it will default to parameter Type's name.</remarks>
        /// <returns>Argument's type name.</returns>
        public static string GetTypeName(this ParameterInfo parameter)
            => parameter.GetCustomAttribute<ArgumentTypeNameAttribute>()?.Name ?? parameter.ParameterType.Name;
    }
}
