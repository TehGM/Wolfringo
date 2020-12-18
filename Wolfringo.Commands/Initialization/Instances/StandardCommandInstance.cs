using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Results;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Commands.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Represents a standard command instance.</summary>
    public class StandardCommandInstance : ICommandInstance
    {
        /// <summary>Text that triggers this command.</summary>
        public string Text { get; }
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

        private readonly Lazy<Regex> _caseSensitiveRegex;
        private readonly Lazy<Regex> _caseInsensitiveRegex;
        private readonly ParameterInfo[] _params;

        /// <summary>Creates a new command instance.</summary>
        /// <param name="text">Text that triggers this command.</param>
        /// <param name="method">Method that will be executed.</param>
        /// <param name="requirements">Execution requirements.</param>
        /// <param name="prefixOverride">Prefix override; null for no overriding.</param>
        /// <param name="prefixRequirementOverride">Prefix requireent override; null for no overriding.</param>
        /// <param name="caseSensitivityOverride">Case sensitivity override; null for no overriding.</param>
        public StandardCommandInstance(string text, MethodInfo method, IEnumerable<ICommandRequirement> requirements, string prefixOverride, PrefixRequirement? prefixRequirementOverride, bool? caseSensitivityOverride)
        {
            this.Text = text.Trim();
            this.Method = method;
            this.Requirements = requirements;
            this.PrefixOverride = prefixOverride;
            this.PrefixRequirementOverride = prefixRequirementOverride;
            this.CaseSensitivityOverride = caseSensitivityOverride;

            string pattern = $@"\G{this.Text}\b(.*)?$";
            this._caseSensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.Singleline));
            this._caseInsensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline));
            this._params = this.Method.GetParameters();
        }

        /// <inheritdoc/>
        public Task<ICommandResult> CheckMatchAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            // ignore non-text messages
            if (!(context.Message is ChatMessage message))
                return FailureResult();
            if (!message.IsText)
                return FailureResult();
            // ignore deleted messages (should never be the case, but let's make sure of it)
            if (message.IsDeleted)
                return FailureResult();
            // ignore own messages
            if (message.SenderID == context.Client.CurrentUserID)
                return FailureResult();

            // check prefix
            bool caseSensitive = this.CaseSensitivityOverride ?? context.Options.CaseSensitivity;
            if (!message.MatchesPrefixRequirement(
                this.PrefixOverride ?? context.Options.Prefix,
                this.PrefixRequirementOverride ?? context.Options.RequirePrefix,
                caseSensitive, out int startIndex))
                return FailureResult();

            // check command text - ironically, I'll use regex here cause it makes things much simpler
            Regex regex = caseSensitive ? _caseSensitiveRegex.Value : _caseInsensitiveRegex.Value;
            Match match = regex.Match(message.Text, startIndex);
            if (match?.Success != true)
                return FailureResult();

            // parse arguments
            IArgumentsParser parser = services.GetRequiredService<IArgumentsParser>();
            string[] args = match.Groups.Count > 1 ? parser.ParseArguments(match.Groups[1].Value, 0).ToArray() : Array.Empty<string>();
            return Task.FromResult<ICommandResult>(StandardCommandMatchResult.Success(args));

            Task<ICommandResult> FailureResult() => Task.FromResult<ICommandResult>(StandardCommandMatchResult.Failure);
        }

        /// <inheritdoc/>
        public async Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult matchResult, object handler, CancellationToken cancellationToken = default)
        {
            // ensure provided check result is valid
            if (matchResult == null)
                throw new ArgumentNullException(nameof(matchResult));
            if (!matchResult.IsSuccess)
                return CommandExecutionResult.Failure;
            if (!(matchResult is StandardCommandMatchResult standardMatchResult))
                throw new ArgumentException($"{nameof(matchResult)} must be of type {typeof(StandardCommandMatchResult).FullName}", nameof(matchResult));

            // run all custom attributes
            foreach (ICommandRequirement check in this.Requirements)
            {
                if (!await check.CheckAsync(context, services, cancellationToken).ConfigureAwait(false))
                    return new CommandExecutionResult(false, new string[] { check.ErrorMessage }, null);
            }

            // build params
            cancellationToken.ThrowIfCancellationRequested();
            IParameterBuilder paramBuilder = services.GetRequiredService<IParameterBuilder>();
            ParameterBuilderValues paramBuilderValues = new ParameterBuilderValues
            {
                Args = standardMatchResult.Arguments,
                ArgumentConverterProvider = services.GetService<IArgumentConverterProvider>(),
                CancellationToken = cancellationToken,
                Context = context,
                Services = services,
                CommandInstance = this
            };
            ParameterBuildingResult paramsResult = await paramBuilder.BuildParamsAsync(_params, paramBuilderValues, cancellationToken).ConfigureAwait(false);
            if (!paramsResult.IsSuccess)
                return paramsResult;

            // execute - if it's a task, await it
            cancellationToken.ThrowIfCancellationRequested();
            if (this.Method.Invoke(handler, paramsResult.Values) is Task returnTask)
                await returnTask.ConfigureAwait(false);
            return CommandExecutionResult.Success;
        }
    }
}
