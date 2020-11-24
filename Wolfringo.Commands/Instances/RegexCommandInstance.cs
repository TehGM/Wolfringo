using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Commands.Results;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Instances
{
    /// <summary>Represents a regex command instance.</summary>
    public class RegexCommandInstance : ICommandInstance
    {
        /// <summary>Regex this command will be triggered for.</summary>
        public Regex Regex { get; }
        /// <summary>Type of the handler containing the command.</summary>
        public Type HandlerType => _method.DeclaringType;

        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _params;
        private readonly PrefixAttribute _prefixAttribute;
        private readonly IEnumerable<CommandRequirementAttribute> _requirements;

        public RegexCommandInstance(Regex regex, MethodInfo method, IEnumerable<CommandRequirementAttribute> requirements)
        {
            this.Regex = regex;
            this._method = method;
            this._params = _method.GetParameters();
            this._requirements = requirements ?? Enumerable.Empty<CommandRequirementAttribute>();

            this._prefixAttribute = this._method.GetCustomAttribute<PrefixAttribute>(true) ?? 
                this.HandlerType.GetCustomAttribute<PrefixAttribute>(true);
        }

        public RegexCommandInstance(Regex regex, MethodInfo method)
            : this(regex, method, Enumerable.Empty<CommandRequirementAttribute>()) { }

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
            if (!message.MatchesPrefixRequirement(
                this._prefixAttribute?.PrefixOverride ?? context.Options.Prefix,
                this._prefixAttribute?.PrefixRequirementOverride ?? context.Options.RequirePrefix,
                context.Options.CaseInsensitive, out int startIndex))
                return FailureResult();

            // perform regex match
            Match match = this.Regex.Match(message.Text, startIndex);
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
            foreach (CommandRequirementAttribute check in _requirements)
            {
                if (!await check.RunAsync(context, cancellationToken).ConfigureAwait(false))
                    return CommandExecutionResult.Failure;
            }

            // build params
            cancellationToken.ThrowIfCancellationRequested();
            object[] paramsValues = new object[_params.Length];
            foreach (ParameterInfo param in _params)
            {
                object value = null;
                if (param.ParameterType.IsAssignableFrom(context.GetType()))
                    value = context;
                else if (param.ParameterType.IsAssignableFrom(typeof(Match)))
                    value = regexMatchResult.RegexMatch;
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
