using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandHandlerProvider
    {
        object GetCommandHandler(CommandAttributeBase commandAttribute, Type handlerType);
    }
}
