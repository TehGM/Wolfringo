using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands;

namespace TehGM.Wolfringo.Examples.HostedCommandsBot
{
    /// <summary>Example message handler.</summary>
    /// <remarks>For more examples on how command handlers work - see SimpleCommandsBot example project!</remarks>
    /*** Example: hidden handler
     * This example shows that this all commands in this handler will be hidden if [Hidden] attribute is set on the handler itself.
     ***/
    [CommandsHandler]
    [Hidden]
    public class HostedCommandsHandler
    {
        /*** Example: Command without arguments.
         * This example shows a simple command, without arguments.
         * This example uses a concrete CommandContext class, and cancellation token to support task cancellation.
         ***/
        [Command("ping")]
        [Summary("Simply sends a response!")]
        public async Task CmdPingAsync(CommandContext context, CancellationToken cancellationToken = default)
        {
            await context.ReplyTextAsync("Pong!", cancellationToken);
        }

        /*** For more examples, see SimpleCommandsBot example project ***/
    }
}
