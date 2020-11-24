using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Results;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Commands.Instances
{
    public class StandardCommandInstance : ICommandInstance
    {
        private static readonly char[] _argSeparators = { ' ' };

        /// <summary>Text this command will be triggered for.</summary>
        public string Text { get; }
        /// <summary>Type of the handler containing the command.</summary>
        public Type HandlerType => _method.DeclaringType;

        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _params;
        private readonly PrefixAttribute _prefixAttribute;
        private readonly IEnumerable<CommandRequirementAttribute> _requirements;
        private readonly Lazy<Regex> _regexCaseSensitive;
        private readonly Lazy<Regex> _regexCaseInsensitive;
        private readonly bool? _caseInsensitive;

        public StandardCommandInstance(string text, bool? caseInsensitive, MethodInfo method, IEnumerable<CommandRequirementAttribute> requirements)
        {
            this.Text = text.Trim();
            this._method = method;
            this._params = _method.GetParameters();
            this._caseInsensitive = caseInsensitive;
            this._requirements = requirements ?? Enumerable.Empty<CommandRequirementAttribute>();

            string pattern = $@"^{this.Text}\b(.*)$";
            this._regexCaseSensitive = new Lazy<Regex>(() => new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.Singleline));
            this._regexCaseInsensitive = new Lazy<Regex>(() => new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline));

            this._prefixAttribute = this._method.GetCustomAttribute<PrefixAttribute>(true) ??
                this.HandlerType.GetCustomAttribute<PrefixAttribute>(true);
        }

        public StandardCommandInstance(string text, bool? caseInsensitive, MethodInfo method)
            : this(text, caseInsensitive, method, Enumerable.Empty<CommandRequirementAttribute>()) { }

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
            bool caseInsensitive = this._caseInsensitive ?? context.Options.CaseInsensitive;
            if (!message.MatchesPrefixRequirement(
                this._prefixAttribute?.PrefixOverride ?? context.Options.Prefix,
                this._prefixAttribute?.PrefixRequirementOverride ?? context.Options.RequirePrefix,
                caseInsensitive, out int startIndex))
                return FailureResult();
            // check command text - ironically, I'll use regex here cause it makes things much simpler
            Regex regex = caseInsensitive ? _regexCaseInsensitive.Value : _regexCaseSensitive.Value;
            Match match = regex.Match(message.Text.Substring(startIndex));
            if (match?.Success != true)
                return FailureResult();
            // parse arguments
            // TODO: allow more advanced scenarios, such as "" - spaces only is just initial
            string[] args = match.Groups[1].Value.Split(_argSeparators, StringSplitOptions.RemoveEmptyEntries);
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
            foreach (CommandRequirementAttribute check in _requirements)
            {
                if (!await check.RunAsync(context, services, cancellationToken).ConfigureAwait(false))
                    return new CommandExecutionResult(false, new string[] { check.ErrorMessage }, null);
            }

            // build params
            cancellationToken.ThrowIfCancellationRequested();
            object[] paramsValues = new object[_params.Length];
            foreach (ParameterInfo param in _params)
            {
                object value = null;
                if (param.ParameterType.IsAssignableFrom(context.GetType()))
                    value = context;
                else if (param.ParameterType.IsAssignableFrom(typeof(string[])))
                    value = standardMatchResult.Arguments;
                else if (param.ParameterType.IsAssignableFrom(context.Message.GetType()))
                    value = context.Message;
                else if (param.ParameterType.IsAssignableFrom(context.Client.GetType()))
                    value = context.Client;
                else if (param.ParameterType.IsAssignableFrom(typeof(CancellationToken)))
                    value = cancellationToken;
                else
                {
                    value = services.GetService(param.ParameterType);
                    if (value == null)
                    {
                        if (param.IsOptional)
                            value = param.HasDefaultValue ? param.DefaultValue : null;
                        else
                            throw new InvalidOperationException($"Unsupported param type: {param.ParameterType.FullName}");
                    }
                }
                paramsValues[param.Position] = value;
            }

            // execute - if it's a task, await it
            cancellationToken.ThrowIfCancellationRequested();
            if (_method.Invoke(handler, paramsValues) is Task returnTask)
                await returnTask.ConfigureAwait(false);
            return CommandExecutionResult.Success;
        }
    }
}
