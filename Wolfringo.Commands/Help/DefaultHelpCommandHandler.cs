﻿using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands.Help
{
    internal class DefaultHelpCommandHandler
    {
        private readonly CommandsService _service;
        private readonly CommandsOptions _options;

        public DefaultHelpCommandHandler(CommandsOptions options, CommandsService service)
        {
            this._options = options;
            this._service = service;
        }

        [Command("help")]
        [Hidden]
        public async Task<ICommandResult> CmdHelpAsync(ICommandContext context, CancellationToken cancellationToken = default)
        {
            if (!this._options.EnableDefaultHelpCommand)
                return CommandExecutionResult.Skip;

            CommandsListBuilder builder = new CommandsListBuilder(this._service);
            builder.PrependedPrefix = this._options.Prefix;
            string result = builder.GetCommandsList();
            if (string.IsNullOrWhiteSpace(result))
                return new CommandExecutionResult(CommandResultStatus.Failure, new string[] { "No unhidden commands found!" }, null);

            await context.ReplyTextAsync(result, cancellationToken).ConfigureAwait(false);
            return CommandExecutionResult.Success;
        }
    }
}