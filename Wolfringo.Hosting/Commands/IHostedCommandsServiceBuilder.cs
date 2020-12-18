using System;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Commands.Parsing;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Builder for hosted commands service.</summary>
    public interface IHostedCommandsServiceBuilder
    {
        /// <summary>Alters configuration of Command Service.</summary>
        IHostedCommandsServiceBuilder Configure(Action<CommandsOptions> configure);
        /// <summary>Alters configuration of Arguments Parser.</summary>
        IHostedCommandsServiceBuilder ConfigureArgumentsParser(Action<ArgumentsParserOptions> configure);
        /// <summary>Alters configuration of Command Initializer Provider.</summary>
        IHostedCommandsServiceBuilder ConfigureCommandInitializerProvider(Action<CommandInitializerProviderOptions> configure);
        /// <summary>Alters configuration of Argument Converter Provider.</summary>
        IHostedCommandsServiceBuilder ConfigureArgumentConverterProvider(Action<ArgumentConverterProviderOptions> configure);
    }
}
