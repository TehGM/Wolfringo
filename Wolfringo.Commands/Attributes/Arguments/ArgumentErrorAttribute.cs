using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Parsing;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Base attribute for all argument error attributes.</summary>
    /// <remarks><para>Decorating a method parameter with this argument will allow specifying message that will be sent to the user if there's any error when building the param.</para>
    /// <para><see cref="TextTemplate"/> can have a few placeholders inside. On runtime, each of these placeholders will be replaced with an actual value. Currently supported placeholders are: <see cref="ArgPlaceholder"/>, <see cref="TypePlaceholder"/>, <see cref="NamePlaceholder"/>, <see cref="MessagePlaceholder"/>, <see cref="SenderNicknamePlaceholder"/>, <see cref="SenderIdPlaceholder"/>, <see cref="BotNicknamePlaceholder"/>, <see cref="BotIdPlaceholder"/>.<br/>
    /// To add new placeholders, create new attributes inheriting from <see cref="ConvertingErrorAttribute"/> or <see cref="MissingErrorAttribute"/>, and override <see cref="ToStringAsync(ICommandContext, string, ParameterInfo, CancellationToken)"/> method. To preserve default placeholders, call <code>base.ToStringAsync()</code> BEFORE your custom placeholders handling.</para></remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class ArgumentErrorAttribute : Attribute, IEquatable<ArgumentErrorAttribute>, IEquatable<string>
    {
        /// <summary>Error message that will be sent as response on error.</summary>
        public string TextTemplate { get; }

        // placeholders
        /// <summary>This placeholder will be replaced with argument value at runtime.</summary>
        public const string ArgPlaceholder = "{{Arg}}";
        /// <summary>This placeholder will be replaced with parameter type at runtime.</summary>
        /// <remarks>Can be changed by using <see cref="ArgumentTypeNameAttribute"/>.</remarks>
        public const string TypePlaceholder = "{{Type}}";
        /// <summary>This placeholder will be replaced with parameter name at runtime.</summary>
        /// <remarks>Can be changed by using <see cref="ArgumentNameAttribute"/>.</remarks>
        public const string NamePlaceholder = "{{Name}}";
        /// <summary>This placeholder will be replaced with received message contents at runtime.</summary>
        public const string MessagePlaceholder = "{{Message}}";
        /// <summary>This placeholder will be replaced with nickname of the user that sent the command at runtime.</summary>
        public const string SenderNicknamePlaceholder = "{{SenderNickname}}";
        /// <summary>This placeholder will be replaced with ID of the user that sent the command at runtime.</summary>
        public const string SenderIdPlaceholder = "{{SenderID}}";
        /// <summary>This placeholder will be replaced with nickname of the bot at runtime.</summary>
        public const string BotNicknamePlaceholder = "{{BotNickname}}";
        /// <summary>This placeholder will be replaced with ID of the bot at runtime.</summary>
        public const string BotIdPlaceholder = "{{BotID}}";
        /// <summary>This placeholder will be replaced with ID of the message recipient (bot ID for PMs, group ID for group messages).</summary>
        public const string RecipientIdPlaceholder = "{{RecipientID}}";
        /// <summary>This placeholder will be replaced with name of the message recipient (bot nickname for PMs, group name for group messages).</summary>
        public const string RecipientNamePlaceholder = "{{RecipientName}}";

        // placeholder regexes
        private const RegexOptions _regexOptions = RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
        private static readonly Lazy<Regex> _argRegex = new Lazy<Regex>(() => new Regex(ArgPlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _typeRegex = new Lazy<Regex>(() => new Regex(TypePlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _nameRegex = new Lazy<Regex>(() => new Regex(NamePlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _messageRegex = new Lazy<Regex>(() => new Regex(MessagePlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _senderNicknameRegex = new Lazy<Regex>(() => new Regex(SenderNicknamePlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _senderIdRegex = new Lazy<Regex>(() => new Regex(SenderIdPlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _botNicknameRegex = new Lazy<Regex>(() => new Regex(BotNicknamePlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _botIdRegex = new Lazy<Regex>(() => new Regex(BotIdPlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _recipientIdRegex = new Lazy<Regex>(() => new Regex(RecipientIdPlaceholder, _regexOptions));
        private static readonly Lazy<Regex> _recipientNameRegex = new Lazy<Regex>(() => new Regex(RecipientNamePlaceholder, _regexOptions));

        /// <summary>Attribute to specify message that will be sent as respone when argument error occurs.</summary>
        /// <param name="messageTemplate">Message that will be sent as response on error.</param>
        public ArgumentErrorAttribute(string messageTemplate) : base()
        {
            this.TextTemplate = messageTemplate;
        }

        /// <inheritdoc/>
        public override string ToString()
            => this.TextTemplate;

        /// <summary>Builds error response for the sent message and arg.</summary>
        /// <param name="arg">Value of argument that failed converting.</param>
        /// <param name="parameter">Parameter the error is for.</param>
        /// <param name="context">Message context that contains the argument.</param>
        /// <param name="cancellationToken">Token to cancel any required server requests with.</param>
        public virtual async Task<string> ToStringAsync(ICommandContext context, string arg, ParameterInfo parameter, CancellationToken cancellationToken = default)
        {
            string result = this.TextTemplate;
            // use regex for replacing - this allows for case insensitive replacements
            result = _argRegex.Value.Replace(result, arg);
            result = _typeRegex.Value.Replace(result, parameter.GetTypeName());
            result = _nameRegex.Value.Replace(result, parameter.GetArgumentName());
            result = _messageRegex.Value.Replace(result, Encoding.UTF8.GetString(context.Message.RawData.ToArray()));
            result = _senderIdRegex.Value.Replace(result, context.Message.SenderID.Value.ToString());
            result = _botIdRegex.Value.Replace(result, context.Client.CurrentUserID.Value.ToString());
            result = _recipientIdRegex.Value.Replace(result, context.Message.RecipientID.ToString());
            // do IndexOf checks for values that are potentially expensive to get
            if (result.IndexOf(SenderNicknamePlaceholder, StringComparison.OrdinalIgnoreCase) != -1)
                result = _senderNicknameRegex.Value.Replace(result, (await context.GetSenderAsync(cancellationToken).ConfigureAwait(false)).Nickname);
            if (result.IndexOf(BotNicknamePlaceholder, StringComparison.OrdinalIgnoreCase) != -1)
                result = _botNicknameRegex.Value.Replace(result, (await context.GetBotProfileAsync(cancellationToken).ConfigureAwait(false)).Nickname);
            if (result.IndexOf(RecipientIdPlaceholder, StringComparison.OrdinalIgnoreCase) != -1)
            {
                string recipientName = context.Message.IsGroupMessage
                    ? (await context.GetRecipientAsync<WolfGroup>(cancellationToken).ConfigureAwait(false)).Name
                    : (await context.GetRecipientAsync<WolfUser>(cancellationToken).ConfigureAwait(false)).Nickname;
                result = _recipientNameRegex.Value.Replace(result, recipientName);
            }
            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is ArgumentErrorAttribute attr)
                return Equals(attr);
            if (obj is string str)
                return Equals(str);
            return false;
        }

        /// <inheritdoc/>
        public bool Equals(ArgumentErrorAttribute other)
            => other != null && TextTemplate == other.TextTemplate;

        /// <inheritdoc/>
        public bool Equals(string other)
            => other != null && TextTemplate == other;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 92862342;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TextTemplate);
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(ArgumentErrorAttribute left, ArgumentErrorAttribute right)
            => EqualityComparer<ArgumentErrorAttribute>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(ArgumentErrorAttribute left, ArgumentErrorAttribute right)
            => !(left == right);
    }
}
