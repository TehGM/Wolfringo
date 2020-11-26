using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Examples.SimpleCommandsBot
{
    /*** Example: transient command handler
     * Transient command handler is stateless. That means, it'll be created just to execute a command method, and destroyed right after.
     * This is good for very simple commands. If you need to use events or any other state, check ExamplePersistentCommandHandler.
     * 
     * Note: if your command handler implements IDisposable, its Dispose() method will be called once command finishes executing.
     ***/
    [CommandHandler]
    class ExampleTransientCommandsHandler
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


        /*** Example: Command with arguments and custom errors.
         * This example shows a simple command, with arguments.
         * Arguments in this example have customized error messages using special attributes.
         * This example uses a ICommandContext interface as abstraction.
         * 
         * Note: custom error messages are optional. If they're missing, default ones will be used.
         ***/
        [Command("delayed ping")]
        public async Task CmdDelayedPingAsync(ICommandContext context,
            [ConvertingError("(n) '{{Arg}}' is not a valid number!")]
            [MissingError("(n) You need to provide delay value!")] int delaySeconds)
        {
            if (delaySeconds <= 0)
                await context.ReplyTextAsync("(n) Delay cannot be less than 1 second!");
            else
            {
                await Task.Delay(delaySeconds * 1000);
                await context.ReplyTextAsync("Pong!");
            }
        }


        /*** Example: Private command with optional arguments.
         * This example shows a simple commands, with a mandatory and an optional argument.
         * Arguments that have defaults set in the method signature will use these defaults if user does not provide a value.
         * This works for services as well - using default value (like null) means they'll be injected if available, and skipped if not available.
         * This example also shows that commands don't need to be public. They cannot be static, but they can be private!
         ***/
        [Command("optionals")]
        private async Task CmdOptionalsAsync(ICommandContext context, bool first, bool second = false)
        {
            await context.ReplyTextAsync($"First: {first}, Second: {second}");
        }

        /*** Example: Regex command.
         * This example shows a regex command, using Regex Match.
         * In addition to regex match, 1st regex group is parsed as a normal argument.
         * As an addition, this command is set to not require prefix in private messages.
         ***/
        [RegexCommand("^hello (.+?)(?:\\s(.*))?$")]
        [Prefix(PrefixRequirement.Group)]
        public async Task CmdHelloAsync(ICommandContext context, Match match, string arg1)
        {
            await context.ReplyTextAsync($"Hello, but {arg1} is not my name!");
            if (match.Groups[2].Success && match.Groups[2].Length > 0)
                await context.ReplyTextAsync($"Remainder: {match.Groups[2].Value}");
        }


        /*** Example: Commands with priority.
         * This example demonstrates use of priority.
         * Command 1 has lower priority than command 2, but use same command text.
         * Priority attribute ensures only the command with highest priority is executed.
         * This can be especially useful with help commands - or regex commands with multiple overloads.
         ***/
        [Command("priority")]
        [Priority(5)]
        public Task CmdPriority5Async(ICommandContext context)
            => context.ReplyTextAsync("Priority 5");
        [Command("priority")]
        [Priority(15)]
        public Task CmdPriority15Async(ICommandContext context)
            => context.ReplyTextAsync("Priority 15");


        /*** Example: Dependency Injection.
         * This example shows that Dependency Injection can be used with commands.
         * You can inject IWolfClient, ChatMessage, IServiceProvider etc.
         * Any service added to the service provider can also be injected, which makes it really useful with .NET Generic Host.
         * In this example, ILogger and CommandsOptions are both resolved from IServiceProvider.
         * 
         * Note: this example shows that command method can return void instead of Task. It is however advised to NOT use async void - for async, use Task.
         ***/
        [Command("dependency injection")]
        public void CmdDependencyInjection(IWolfClient client, ChatMessage message, IServiceProvider services, ILogger log, CommandsOptions options)
        {
            log.LogInformation("Current user: {UserID}", client.CurrentUserID.Value);
            log.LogInformation("Message contents: {Message}", message.Text);
            log.LogInformation("Type of IServiceProvider: {Type}", services.GetType().Name);
            log.LogInformation("Prefix: {Prefix}", options.Prefix);
        }


        /*** Example: Privileges requirements.
         * This example shows admin only command. With [RequireGroupAdmin], command will only work if user executing the command is owner or admin.
         * There are other similar attributes: [RequireGroupOwner] and [RequireGroupMod].
         * There also are commands that check if the bot has privileges in group. These are [RequireBotGroupOwner], [RequireBotGroupAdmin] and [RequireBotGroupMod].
         * 
         * Note: [GroupOnly] is optional with [RequireGroupAdmin] etc - group privileges requirements make commands group-only by default.
         * Note: If privilege requirement has IgnoreInPrivate set to false, command will skip check and work in private. Example: [RequireGroupAdmin(IgnoreInPrivate = true)]
         * Note: To make command PM only, use [PrivateOnly] attribute.
         ***/
        [Command("admin only")]
        [RequireGroupAdmin]
        [GroupOnly]
        public async Task CmdAdminOnly(ICommandContext context)
        {
            await context.ReplyTextAsync("You can execute this command!");
        }
    }
}
