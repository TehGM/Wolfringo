using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Extensions for command initializer maps.</summary>
    public static class CommandInitializerMapExtensions
    {
        /// <summary>Gets a command initializer for the command type.</summary>
        /// <typeparam name="T">Type of the command.</</typeparam>
        /// <returns>An initializer for provided command type.</returns>
        public static ICommandInitializer GetMappedInitializer<T>(this ICommandInitializerMap map) where T : CommandAttributeBase
            => map.GetInitializer(typeof(T));
    }
}
