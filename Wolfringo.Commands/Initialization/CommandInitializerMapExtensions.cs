namespace TehGM.Wolfringo.Commands.Initialization
{
    public static class CommandInitializerMapExtensions
    {
        public static ICommandInitializer GetMappedInitializer<T>(this ICommandInitializerMap map)
            => map.GetMappedInitializer(typeof(T));
    }
}
