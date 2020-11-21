namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Extensions for command initializer maps.</summary>
    public static class CommandInitializerMapExtensions
    {
        /// <summary>Gets a command initializer for the command type.</summary>
        /// <typeparam name="T">Type of the command.</</typeparam>
        /// <returns>An initializer for provided command type.</returns>
        public static ICommandInitializer GetMappedInitializer<T>(this ICommandInitializerMap map) where T : CommandAttributeBase
            => map.GetMappedInitializer(typeof(T));

        /// <summary>Maps a command type to an initializer.</summary>
        /// <typeparam name="T">Type of the command.</typeparam>
        /// <param name="commandAttributeType">Type of the command.</param>
        /// <param name="initializer">Initializer to use for that command type.</param>
        public static void MapInitializer<T>(this ICommandInitializerMap map, ICommandInitializer initializer) where T : CommandAttributeBase
            => map.MapInitializer(typeof(T), initializer);
    }
}
