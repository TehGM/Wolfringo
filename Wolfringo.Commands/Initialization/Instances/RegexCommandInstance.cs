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
using Microsoft.Extensions.DependencyInjection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Represents a regex command instance.</summary>
    public class RegexCommandInstance : CommandInstanceBase, ICommandInstance
    {
        /// <summary>Regex pattern that triggers this command.</summary>
        public string Pattern { get; }

        private readonly Lazy<Regex> _caseSensitiveRegex;
        private readonly Lazy<Regex> _caseInsensitiveRegex;

        /// <summary>Creates a new command instance.</summary>
        /// <param name="pattern">Regex pattern that triggers this command.</param>
        /// <param name="regexOptions">Regex options to build Regex with.</param>
        /// <param name="method">Method that will be executed.</param>
        /// <param name="requirements">Execution requirements.</param>
        /// <param name="prefixOverride">Prefix override; null for no overriding.</param>
        /// <param name="prefixRequirementOverride">Prefix requireent override; null for no overriding.</param>
        /// <param name="caseSensitivityOverride">Case sensitivity override; null for no overriding.</param>
        public RegexCommandInstance(string pattern, RegexOptions regexOptions, MethodInfo method, IEnumerable<ICommandRequirement> requirements, string prefixOverride, PrefixRequirement? prefixRequirementOverride, bool? caseSensitivityOverride)
            : base(method, requirements, prefixOverride, prefixRequirementOverride, caseSensitivityOverride)
        {
            this.Pattern = pattern;

            this._caseSensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, regexOptions & ~RegexOptions.IgnoreCase));
            this._caseInsensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, regexOptions | RegexOptions.IgnoreCase));
        }

        /// <inheritdoc/>
        public Task<ICommandResult> CheckMatchAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            // perform base checks
            if (!base.CheckMatch(context, out int startIndex, out CommandContextOptions options))
                return SkipResult();

            // perform regex match
            Regex regex = options.CaseSensitivity ? _caseSensitiveRegex.Value : _caseInsensitiveRegex.Value;
            Match match = regex.Match(((ChatMessage)context.Message).Text, startIndex);
            if (match?.Success != true)
                return SkipResult();
            return Task.FromResult<ICommandResult>(RegexCommandMatchResult.Success(match, options));

            Task<ICommandResult> SkipResult() => Task.FromResult<ICommandResult>(RegexCommandMatchResult.Skip);
        }

        /// <inheritdoc/>
        public async Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult matchResult, object handler, CancellationToken cancellationToken = default)
        {
            // ensure provided check result is valid
            if (matchResult == null)
                throw new ArgumentNullException(nameof(matchResult));
            if (matchResult.Status != CommandResultStatus.Success)
                throw new InvalidOperationException("Cannot run command - command match has failed.");
            if (!(matchResult is RegexCommandMatchResult regexMatchResult))
                throw new ArgumentException($"{nameof(matchResult)} must be of type {typeof(RegexCommandMatchResult).FullName}", nameof(matchResult));

            // run all custom attributes
            foreach (ICommandRequirement check in this.Requirements)
            {
                ICommandResult result = await check.CheckAsync(context, services, cancellationToken).ConfigureAwait(false);
                if (result.Status != CommandResultStatus.Success)
                    return result;
            }

            // build params
            cancellationToken.ThrowIfCancellationRequested();
            ParameterBuilderValues paramBuilderValues = new ParameterBuilderValues
            {
                Args = regexMatchResult.RegexMatch.Groups.Cast<Group>().Skip(1)
                    .Select(s => s.Value ?? string.Empty).ToArray(),
                ArgsText = regexMatchResult.RegexMatch.Value,
                ArgumentConverterProvider = services.GetService<IArgumentConverterProvider>(),
                CancellationToken = cancellationToken,
                Context = context,
                Services = services,
                CommandInstance = this,
                Options = regexMatchResult.Options,
                AdditionalObjects = new object[] { regexMatchResult.RegexMatch }
            };

            // delegate invokation to shared base class
            return await base.InvokeCommandAsync(paramBuilderValues, services, handler, cancellationToken);
        }
    }
}
