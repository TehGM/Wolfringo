﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Represents any special validation that message needs to pass for command to be executed.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class CommandRequirementAttribute : Attribute, ICommandRequirement
    {
        /// <summary>The message that bot should reply with if requirement was not fulfilled.</summary>
        public string ErrorMessage { get; set; }
        /// <summary>Whether check fail should cause Command execution to be aborted completely. True by default.</summary>
        public bool AbortOnFail { get; set; } = true;
        /// <summary>Whether skip result should cause <see cref="ErrorMessage"/> to be sent. Defaults to false.</summary>
        /// <remarks>Has no effect if <see cref="AbortOnFail"/> is set to false;</remarks>
        public bool SendMessageWhenSkipping { get; set; } = false;

        /// <summary>Checks requirement.</summary>
        /// <param name="context">Command to check the requirement for.</param>
        /// <param name="services">Services that can be used during requirement checks.</param>
        /// <param name="cancellationToken">Token for cancelling the task.</param>
        /// <returns>True if requirement was fullfilled; otherwise false.</returns>
        public abstract Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default);

        /// <summary>Standard success result.</summary>
        protected ICommandResult SuccessResult { get; } = CommandRequirementsResult.Success();
        /// <summary>Standard failure result.</summary>
        /// <remarks>This result will automatically use <see cref="ErrorMessage"/> if it's set.</remarks>
        protected ICommandResult FailureResult =>
            this.AbortOnFail ? CommandRequirementsResult.Failure(this.ErrorMessage) :
            (this.SendMessageWhenSkipping ? CommandRequirementsResult.Skip(this.ErrorMessage) : CommandRequirementsResult.Skip());

        /// <summary>Converts a simple boolean to a proper command result.</summary>
        /// <param name="isSuccess">Whether success or failure result should be returned.</param>
        /// <returns>Command result based on <paramref name="isSuccess"/>.</returns>
        protected ICommandResult ResultFromBoolean(bool isSuccess)
            => isSuccess ? this.SuccessResult : this.FailureResult;
    }
}
