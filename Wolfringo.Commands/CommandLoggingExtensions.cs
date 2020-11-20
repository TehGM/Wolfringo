using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TehGM.Wolfringo.Commands
{
    public static class CommandLoggingExtensions
    {
        public static IDisposable BeginCommandScope(this ILogger log, ICommandContext context, object handler = null, [CallerMemberName] string cmdName = null)
            => BeginCommandScope(log, context, handler?.GetType(), cmdName);

        public static IDisposable BeginCommandScope(this ILogger log, ICommandContext context, Type handlerType = null, [CallerMemberName] string cmdName = null)
        {
            Dictionary<string, object> state = new Dictionary<string, object>
            {
                { "Command.SenderID", context.Message.SenderID.Value },
                { "Command.MessageID", context.Message.Timestamp.ToString() },
                { "Command.RecipientID", context.Message.RecipientID },
                { "Command.RecipientType", context.Message.IsGroupMessage ? "Group" : "Private" }
            };
            if (!string.IsNullOrWhiteSpace(cmdName))
                state.Add("Command.Method", cmdName);
            if (handlerType != null)
                state.Add("Command.Handler", handlerType.Name);
            return log.BeginScope(state);
        }
    }
}
