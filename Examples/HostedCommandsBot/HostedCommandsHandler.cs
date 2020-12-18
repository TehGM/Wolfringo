using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands;

namespace TehGM.Wolfringo.Examples.HostedCommandsBot
{
    /// <summary>Example message handler.</summary>
    /// <remarks>For more examples on how command handlers work - see SimpleCommandsBot example project!</remarks>
    [CommandsHandler]
    public class HostedCommandsHandler
    {
        /*** Example: Command without arguments.
         * This example shows a simple command, without arguments.
         * This example uses a concrete CommandContext class, and cancellation token to support task cancellation.
         ***/
        [Command("ping")]
        public async Task CmdPingAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            await context.ReplyTextAsync("Pong!", cancellationToken);
        }

        /*** For more examples, see SimpleCommandsBot example project ***/
    }
}
