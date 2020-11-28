﻿namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>A service that deals with resolving Handler instance to run command in.</summary>
    public interface ICommandsHandlerProvider
    {
        /// <summary>Resolves a command handler instance.</summary>
        /// <param name="descriptor">Descriptor of the command instance.</param>
        /// <returns>A result of the command handler initialization.</returns>
        ICommandsHandlerProviderResult GetCommandHandler(ICommandInstanceDescriptor descriptor);
    }
}