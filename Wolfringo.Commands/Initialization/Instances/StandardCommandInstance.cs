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
    public class StandardCommandInstance : CommandInstanceBase, ICommandInstance
    {
        /// <summary>Text that triggers this command.</summary>
        public string Text { get; }

        private readonly Lazy<Regex> _caseSensitiveRegex;
        private readonly Lazy<Regex> _caseInsensitiveRegex;

        /// <summary>Creates a new command instance.</summary>
        /// <param name="text">Text that triggers this command.</param>
        /// <param name="method">Method that will be executed.</param>
        /// <param name="requirements">Execution requirements.</param>
        /// <param name="prefixOverride">Prefix override; null for no overriding.</param>
        /// <param name="prefixRequirementOverride">Prefix requireent override; null for no overriding.</param>
        /// <param name="caseSensitivityOverride">Case sensitivity override; null for no overriding.</param>
        public StandardCommandInstance(string text, MethodInfo method, IEnumerable<ICommandRequirement> requirements, string prefixOverride, PrefixRequirement? prefixRequirementOverride, bool? caseSensitivityOverride)
            : base(method, requirements, prefixOverride, prefixRequirementOverride, caseSensitivityOverride)
        {
            this.Text = text.Trim();

            string pattern = $@"\G{this.Text}\b(.*)?$";
            this._caseSensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.Singleline));
            this._caseInsensitiveRegex = new Lazy<Regex>(() => new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline));
        }

        /// <inheritdoc/>
        public Task<ICommandResult> CheckMatchAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            // perform base checks
            if (!base.CheckMatch(context, out int startIndex, out CommandContextOptions options))
                return SkipResult();

            // check command text - ironically, I'll use regex here cause it makes things much simpler
            Regex regex = options.CaseSensitivity ? _caseSensitiveRegex.Value : _caseInsensitiveRegex.Value;
            Match match = regex.Match(((ChatMessage)context.Message).Text, startIndex);
            if (match?.Success != true)
                return SkipResult();

            string argumentsText = match.Groups.Count > 1 ? match.Groups[1].Value : string.Empty;
            return Task.FromResult<ICommandResult>(StandardCommandMatchResult.Success(argumentsText, options));

            Task<ICommandResult> SkipResult() => Task.FromResult<ICommandResult>(StandardCommandMatchResult.Skip);
        }

        /// <inheritdoc/>
        public async Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult matchResult, object handler, CancellationToken cancellationToken = default)
        {
            // ensure provided check result is valid
            if (matchResult == null)
                throw new ArgumentNullException(nameof(matchResult));
            if (matchResult.Status != CommandResultStatus.Success)
                throw new InvalidOperationException("Cannot run command - command match has failed.");
            if (!(matchResult is StandardCommandMatchResult standardMatchResult))
                throw new ArgumentException($"{nameof(matchResult)} must be of type {typeof(StandardCommandMatchResult).FullName}", nameof(matchResult));

            // run all custom attributes
            foreach (ICommandRequirement check in this.Requirements)
            {
                ICommandResult result = await check.CheckAsync(context, services, cancellationToken).ConfigureAwait(false);
                if (result.Status != CommandResultStatus.Success)
                    return result;
            }

            // parse arguments
            IArgumentsParser parser = services.GetRequiredService<IArgumentsParser>();
            string[] args = !string.IsNullOrWhiteSpace(standardMatchResult.ArgumentsText) ? parser.ParseArguments(standardMatchResult.ArgumentsText, 0).ToArray() : Array.Empty<string>();

            // build params
            cancellationToken.ThrowIfCancellationRequested();
            ParameterBuilderValues paramBuilderValues = new ParameterBuilderValues
            {
                Args = args,
                ArgsText = standardMatchResult.ArgumentsText,
                ArgumentConverterProvider = services.GetService<IArgumentConverterProvider>(),
                CancellationToken = cancellationToken,
                Context = context,
                Services = services,
                Options = standardMatchResult.Options,
                CommandInstance = this
            };

            // delegate invokation to shared base class
            return await base.InvokeCommandAsync(paramBuilderValues, services, handler, cancellationToken);
        }
    }
}
