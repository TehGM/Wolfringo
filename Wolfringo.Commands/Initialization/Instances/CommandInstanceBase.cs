using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TehGM.Wolfringo.Commands.Parsing;
using TehGM.Wolfringo.Commands.Results;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Base class with shared functionality for <see cref="RegexCommandInstance"/> and <see cref="StandardCommandInstance"/>.</summary>
    /// <remarks>This class was created for internal use.<br/>
    /// Command instances might inherit from this class, but this is not required. If you want to use a shared abstraction for command instances, use <see cref="ICommandInstance"/> instead.</remarks>
    public abstract class CommandInstanceBase
    {
        /// <summary>Method that will be executed.</summary>
        public MethodInfo Method { get; }
        /// <summary>Execution requirements.</summary>
        public IEnumerable<ICommandRequirement> Requirements { get; }
        /// <summary>Prefix override; null for no overriding.</summary>
        public string PrefixOverride { get; }
        /// <summary>Prefix requireent override; null for no overriding.</summary>
        public PrefixRequirement? PrefixRequirementOverride { get; }
        /// <summary>Case sensitivity override; null for no overriding.</summary>
        public bool? CaseSensitivityOverride { get; }
        /// <summary>Type of the handler containing the command.</summary>
        public Type HandlerType => this.Method.DeclaringType;
        /// <summary>All command method's parameters.</summary>
        protected ParameterInfo[] Parameters { get; }

        /// <summary>Creates a new command instance.</summary>
        /// <param name="method">Method that will be executed.</param>
        /// <param name="requirements">Execution requirements.</param>
        /// <param name="prefixOverride">Prefix override; null for no overriding.</param>
        /// <param name="prefixRequirementOverride">Prefix requireent override; null for no overriding.</param>
        /// <param name="caseSensitivityOverride">Case sensitivity override; null for no overriding.</param>
        public CommandInstanceBase(MethodInfo method, IEnumerable<ICommandRequirement> requirements, string prefixOverride, PrefixRequirement? prefixRequirementOverride, bool? caseSensitivityOverride)
        {
            this.Method = method;
            this.Requirements = requirements;
            this.PrefixOverride = prefixOverride;
            this.PrefixRequirementOverride = prefixRequirementOverride;
            this.CaseSensitivityOverride = caseSensitivityOverride;

            this.Parameters = method.GetParameters();
        }

        /// <summary>Performs shared match checks.</summary>
        /// <param name="context">Command context to check.</param>
        /// <param name="startIndex">The index of command start (after prefix).</param>
        /// <param name="caseSensitive">Whether checks should be done in a case sensitive manner.</param>
        /// <returns>True if all checks passed; otherwise false.</returns>
        protected bool CheckMatch(ICommandContext context, out int startIndex, out bool caseSensitive)
        {
            caseSensitive = this.CaseSensitivityOverride ?? context.Options.CaseSensitivity;
            startIndex = default;

            // ignore non-text messages
            if (!(context.Message is ChatMessage message))
                return false;
            if (!message.IsText)
                return false;
            // ignore deleted messages (should never be the case, but let's make sure of it)
            if (message.IsDeleted)
                return false;
            // ignore own messages
            if (message.SenderID == context.Client.CurrentUserID)
                return false;
            // check prefix
            if (!message.MatchesPrefixRequirement(
                this.PrefixOverride ?? context.Options.Prefix,
                this.PrefixRequirementOverride ?? context.Options.RequirePrefix,
                caseSensitive, out startIndex))
                return false;

            return true;
        }

        /// <summary>Builds command parameters and invokes the method.</summary>
        /// <param name="parameterBuilderValues">Values for parameter builder.</param>
        /// <param name="services">Services provider for injecting parameters into command method.</param>
        /// <param name="handler">Handler object to execute the command method in.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the execution.</param>
        /// <returns>Result of the execution.</returns>
        protected async Task<ICommandResult> InvokeCommandAsync(ParameterBuilderValues parameterBuilderValues, IServiceProvider services, object handler, CancellationToken cancellationToken)
        {
            IParameterBuilder paramBuilder = services.GetRequiredService<IParameterBuilder>();
            ParameterBuildingResult paramsResult = await paramBuilder.BuildParamsAsync(this.Parameters, parameterBuilderValues, cancellationToken).ConfigureAwait(false);
            if (paramsResult.Status != CommandResultStatus.Success)
                return paramsResult;

            // execute - if it's a task, await it
            // also check if it's ICommandResult - if so, return it
            cancellationToken.ThrowIfCancellationRequested();
            object invokedMethod = this.Method.Invoke(handler, paramsResult.Values);
            if (invokedMethod is Task<ICommandResult> returnTaskWithResult)
                return await returnTaskWithResult.ConfigureAwait(false);
            else if (invokedMethod is ICommandResult returnResult)
                return returnResult;
            else if (invokedMethod is Task returnTask)
                await returnTask.ConfigureAwait(false);

            return CommandExecutionResult.Success;
        }
    }
}
