namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>Extensions for defaul Argument Converter Providers.</summary>
    public static class ArgumentConverterProviderExtensions
    {
        /// <summary>Maps an argument type to a converter.</summary>
        /// <remarks><para>Using this method, it is possible to override converter for specified enum.<br/>
        /// By default, <see cref="Map"/> is checked before attempting to parse enum. If a specific enum is mapped, it'll be found in map and use the converter before default enum converter is used.</para>
        /// <para>Providing a converter for an already mapped type will overwrite the mapped converter.</para></remarks>
        /// <typeparam name="T">Type of the parameter.</typeparam>
        /// <param name="converter">Converter to use for that parameter type.</param>
        public static void MapConverter<T>(this ArgumentConverterProvider provider, IArgumentConverter converter)
            => provider.MapConverter(typeof(T), converter);
    }
}
