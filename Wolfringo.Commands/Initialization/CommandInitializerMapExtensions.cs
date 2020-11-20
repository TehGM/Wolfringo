namespace TehGM.Wolfringo.Commands.Initialization
{
    public static class CommandInitializerMapExtensions
    {
        public static ICommandInitializer GetMappedInitializer<T>(this ICommandInitializerMap map)
            => map.GetMappedInitializer(typeof(T));

        public static void MapInitializer<T>(this ICommandInitializerMap map, ICommandInitializer initializer) where T : CommandAttributeBase
            => map.MapInitializer(typeof(T), initializer);
    }
}
