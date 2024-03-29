﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Represents any special validation that message needs to pass for command to be executed.</summary>
    public interface ICommandRequirement
    {
        /// <summary>Checks requirement.</summary>
        /// <param name="context">Command to check the requirement for.</param>
        /// <param name="services">Services that can be used during requirement checks.</param>
        /// <param name="cancellationToken">Token for cancelling the task.</param>
        /// <returns>True if requirement was fullfilled; otherwise false.</returns>
        Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default);
    }
}
