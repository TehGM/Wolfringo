using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands.Help
{
    internal class DefaultHelpCommandHandler
    {
        private readonly ICommandsService _service;
        private readonly CommandsOptions _options;

        public DefaultHelpCommandHandler(CommandsOptions options, ICommandsService service)
        {
            this._options = options;
            this._service = service;
        }

        [Command("help")]
        [Hidden]
        [Priority(int.MinValue)]
        public async Task<ICommandResult> CmdHelpAsync(ICommandContext context, CancellationToken cancellationToken = default)
        {
            if (!this._options.EnableDefaultHelpCommand)
                return CommandExecutionResult.Skip;

            CommandsListBuilder builder = new CommandsListBuilder(this._service.Commands);
            builder.PrependedPrefix = this._options.Prefix;
            builder.SpaceCategories = true;
            builder.ListCommandsWithoutSummaries = true;

            string result = builder.GetCommandsList();
            if (string.IsNullOrWhiteSpace(result))
                return new CommandExecutionResult(CommandResultStatus.Failure, new string[] { "No commands found!" });

            await context.ReplyTextAsync(result, cancellationToken).ConfigureAwait(false);
            return CommandExecutionResult.Success;
        }
    }
}
