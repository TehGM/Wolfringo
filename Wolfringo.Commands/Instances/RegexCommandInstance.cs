using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Commands.Results;
using System.Collections.Generic;
using TehGM.Wolfringo.Commands.Parsing;

namespace TehGM.Wolfringo.Commands.Instances
{
    /// <summary>Represents a regex command instance.</summary>
    public class RegexCommandInstance : ICommandInstance
    {
        /// <summary>Regex pattern that triggers this command.</summary>
        public string Pattern { get; }
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
        /// <param name="pattern">Regex pattern that triggers this command.</param>
        /// <param name="regexOptions">Regex options to build Regex with.</param>
        /// <param name="method">Method that will be executed.</param>
        /// <param name="requirements">Execution requirements.</param>
        /// <param name="prefixOverride">Prefix override; null for no overriding.</param>
        /// <param name="prefixRequirementOverride">Prefix requireent override; null for no overriding.</param>
        /// <param name="caseSensitivityOverride">Case sensitivity override; null for no overriding.</param>
        public RegexCommandInstance(string pattern, RegexOptions regexOptions, MethodInfo method, IEnumerable<ICommandRequirement> requirements, string prefixOverride, PrefixRequirement? prefixRequirementOverride, bool? caseSensitivityOverride)
        {
            this.Pattern = pattern;
            this.Method = method;
            this.Requirements = requirements;
            this.PrefixOverride = prefixOverride;
            this.PrefixRequirementOverride = prefixRequirementOverride;
            this.CaseSensitivityOverride = caseSensitivityOverride;

            this._caseSensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, regexOptions & ~RegexOptions.IgnoreCase));
            this._caseInsensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, regexOptions | RegexOptions.IgnoreCase));
            this._params = method.GetParameters();
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

            // perform regex match
            Regex regex = caseSensitive ? _caseSensitiveRegex.Value : _caseInsensitiveRegex.Value;
            Match match = regex.Match(message.Text, startIndex);
            if (match?.Success != true)
                return FailureResult();
            return Task.FromResult<ICommandResult>(RegexCommandMatchResult.Success(match));

            Task<ICommandResult> FailureResult() => Task.FromResult<ICommandResult>(RegexCommandMatchResult.Failure);
        }

        /// <inheritdoc/>
        public async Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult matchResult, object handler, CancellationToken cancellationToken = default)
        {
            // ensure provided check result is valid
            if (matchResult == null)
                throw new ArgumentNullException(nameof(matchResult));
            if (!matchResult.IsSuccess)
                return CommandExecutionResult.Failure;
            if (!(matchResult is RegexCommandMatchResult regexMatchResult))
                throw new ArgumentException($"{nameof(matchResult)} must be of type {typeof(RegexCommandMatchResult).FullName}", nameof(matchResult));

            // run all custom attributes
            foreach (ICommandRequirement check in this.Requirements)
            {
                if (!await check.CheckAsync(context, services, cancellationToken).ConfigureAwait(false))
                    return new CommandExecutionResult(false, new string[] { check.ErrorMessage }, null);
            }

            // build params
            cancellationToken.ThrowIfCancellationRequested();
            ParameterBuilderValues paramBuilderValues = new ParameterBuilderValues
            {
                Args = regexMatchResult.RegexMatch.Groups.Cast<Group>().Skip(1)
                    .Select(s => s.Value ?? string.Empty).ToArray(),
                ArgumentConverterProvider = (IArgumentConverterProvider)services.GetService(typeof(IArgumentConverterProvider)),
                CancellationToken = cancellationToken,
                Context = context,
                Services = services,
                AdditionalObjects = new object[] { regexMatchResult.RegexMatch }
            };
            object[] paramsValues = ParameterBuilder.BuildParamsAsync(_params, paramBuilderValues);

            // execute - if it's a task, await it
            cancellationToken.ThrowIfCancellationRequested();
            if (this.Method.Invoke(handler, paramsValues) is Task returnTask)
                await returnTask.ConfigureAwait(false);
            return CommandExecutionResult.Success;
        }
    }
}
