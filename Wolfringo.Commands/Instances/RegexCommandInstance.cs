using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands.Instances
{
    /// <summary>Represents a regex command instance.</summary>
    public class RegexCommandInstance : ICommandInstance
    {
        public Regex Regex { get; }
        public Type HandlerType => _method.DeclaringType;
        public string HandlerMethodName => _method.Name;

        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _params;
        private readonly object _handler;
        private readonly PrefixAttribute _prefixAttribute;

        public RegexCommandInstance(Regex regex, MethodInfo method, object handler)
        {
            this.Regex = regex;
            this._method = method;
            this._params = _method.GetParameters();
            this._handler = handler;

            this._prefixAttribute = this._method.GetCustomAttribute<PrefixAttribute>(true) ?? 
                this.HandlerType.GetCustomAttribute<PrefixAttribute>(true);
        }

        public Task<ICommandResult> CheckShouldRunAsync(ICommandContext context, CancellationToken cancellationToken = default)
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
            return Task.FromResult<ICommandResult>(RegexCommandCheckResult.Success(match));

            Task<ICommandResult> FailureResult() => Task.FromResult<ICommandResult>(RegexCommandCheckResult.Failure);
        }

        public async Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult checkResult, CancellationToken cancellationToken = default)
        {
            // ensure provided check result is valid
            if (checkResult == null)
                throw new ArgumentNullException(nameof(checkResult));
            if (!checkResult.IsSuccess)
                return null;
            if (!(checkResult is RegexCommandCheckResult regexCheckResult))
                throw new ArgumentException($"{nameof(checkResult)} must be of type {typeof(RegexCommandCheckResult).FullName}", nameof(checkResult));

            // build params
            cancellationToken.ThrowIfCancellationRequested();
            object[] paramsValues = new object[_params.Length];
            foreach (ParameterInfo param in _params)
            {
                object value = null;
                if (param.ParameterType.IsAssignableFrom(context.GetType()))
                    value = context;
                else if (param.ParameterType.IsAssignableFrom(typeof(Match)))
                    value = regexCheckResult.RegexMatch;
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
            if (_method.Invoke(_handler, paramsValues) is Task returnTask)
                await returnTask.ConfigureAwait(false);
            return CommandExecutionResult.Success;
        }
    }
}
