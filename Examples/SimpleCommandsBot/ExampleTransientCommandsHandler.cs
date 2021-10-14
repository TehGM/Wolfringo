using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Examples.SimpleCommandsBot
{
    /*** Example: transient command handler
     * Transient command handler is stateless. That means, it'll be created just to execute a command method, and destroyed right after.
     * This is good for very simple commands. If you need to use events or any other state, check ExamplePersistentCommandsHandler.
     * 
     * Note: if your command handler implements IDisposable, its Dispose() method will be called once command finishes executing.
     ***/
    [CommandsHandler]
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
         * This example also shows a perfect use case for [DisplayName] attribute.
         * 
         * Note: custom error messages are optional. If they're missing, default ones will be used.
         ***/
        [Command("delayed ping")]
        [DisplayName("delayed ping <seconds>")]
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


        /*** Example: Command with arguments groups and catch-all.
         * By defaults, arguments for a standard command is split using space.
         * However it is possible to capture arguments as a group.
         * Groups are defined by user - if user types something wrapped in a [], () or "", it'll be treated as a group.
         * For example: !word "hello, I am a super bot" --- argument 1 will be: hello, I am a super bot
         * 
         * You can also capture all arguments. If any of the parameters is string[], all of the arguments will be put into it.
         * For example: !word hello [group name] --- argument 1 will be: hello; string[] will contain "hello" and "group name"
         * 
         * Note: with [RegexCommand], default splitting and grouping rules do not apply. For RegexCommands, Regex Groups are used instead.
         * Note: string[] does not make say optional - it is still required. If you want to make it optional, look below
         * Note: group markers - (), [], "" - will not be included in any of arguments. If you want to include them, please use [RegexCommand] with one group.
         ***/
        [Command("word")]
        [DisplayName("word <word>")]
        public async Task CmdSayAsync(CommandContext context, string say, string[] catchAll)
        {
            await context.ReplyTextAsync($"You asked me to one word say {say}");
            if (catchAll.Length > 1)
                await context.ReplyTextAsync($"BUT I WILL SAY {string.Join(' ', catchAll)}");
        }

        /*** Example: Command with injecting command text.
         * You can inject raw arguments text by using [ArgumentsText] attribute on a string parameter.
         * This text will not include prefix or command name.
         * 
         * Note: with regex commands, it'll include entire match value. Regex commands have no way to separate command name and arguments.
         ***/
        [Command("say")]
        private async Task CmdArgumentsTextAsync(CommandContext context, [ArgumentsText] string text)
        {
            await context.ReplyTextAsync($"Okay, fine... {text}");
        }

        /*** Example: injection command options.
         * You can use ICommandOptions as one of the parameters.
         * These options will retrieve global commands options as configured when starting bot, and then apply overrides from attributes like [Prefix].
         ***/
        [Command("prefix info")]
        [Prefix("#")]
        private async Task CmdPrefixInfoAsync(CommandContext context, ICommandOptions options)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Prefix: {options.Prefix}");    // will be # cause of [Prefix] attribute
            builder.AppendLine($"Prefix required: {options.RequirePrefix}");
            builder.AppendLine($"Case sensitive: {options.CaseSensitivity}");
            await context.ReplyTextAsync(builder.ToString());
        }

        /*** Example: Private command with optional arguments.
         * This example shows a simple commands, with a mandatory and an optional argument.
         * Arguments that have defaults set in the method signature will use these defaults if user does not provide a value.
         * This works for services as well - using default value (like null) means they'll be injected if available, and skipped if not available.
         * This example also shows that commands don't need to be public. They cannot be static, but they can be private!
         * 
         * This and next examples also show usage of [HelpCategory] - they'll be grouped together.
         ***/
        [Command("optionals")]
        [DisplayName("optionals <first> {second}")]
        [HelpCategory("Category Example")]
        private async Task CmdOptionalsAsync(CommandContext context, bool first, bool second = false)
        {
            await context.ReplyTextAsync($"First: {first}, Second: {second}");
        }

        /*** Example: Regex command.
         * This example shows a regex command, using Regex Match.
         * In addition to regex match, 1st regex group is parsed as a normal argument.
         * As an addition, this command is set to not require prefix in private messages.
         * 
         * This command also shows another example where [DisplayName] could be required:
         * Regex commands will show as confusing on help list, unless changed with [DisplayName].
         * 
         * This and previous examples also show usage of [HelpCategory] - they'll be grouped together.
         ***/
        [RegexCommand("^hello (.+?)(?:\\s(.*))?$")]
        [DisplayName("hello <name>")]
        [Prefix(PrefixRequirement.Group)]
        [HelpCategory("Category Example")]
        public async Task CmdHelloAsync(CommandContext context, Match match, string arg1)
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
         * 
         * This example also shows [Hidden] attribute:
         * Because 2nd command is marked as hidden, it'll not be shown on the help list!
         * This command will also appear on the top of the list, as it has the highest priority.
         ***/
        [Command("priority")]
        [Priority(5)]
        public Task CmdPriority5Async(CommandContext context)
            => context.ReplyTextAsync("Priority 5");
        [Command("priority")]
        [Priority(15)]
        [Hidden]
        public Task CmdPriority15Async(CommandContext context)
            => context.ReplyTextAsync("Priority 15");


        /*** Example: Dependency Injection.
         * This example shows that Dependency Injection can be used with commands.
         * You can inject IWolfClient, ChatMessage, IServiceProvider, ICommandInstance etc.
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
         * This example also shows usage of [Summary] - a short description of the command will be shown on help list!
         * 
         * Note: [GroupOnly] is optional with [RequireGroupAdmin] etc - group privileges requirements make commands group-only by default.
         * Note: If privilege requirement has IgnoreInPrivate set to false, command will skip check and work in private. Example: [RequireGroupAdmin(IgnoreInPrivate = true)]
         * Note: To make command PM only, use [PrivateOnly] attribute.
         ***/
        [Command("admin only")]
        [GroupOnly]
        [RequireGroupAdmin]
        [Summary("This command will only work if you're admin!")]
        public async Task CmdAdminOnlyAsync(CommandContext context)
        {
            await context.ReplyTextAsync("You can execute this command!");
        }


        /*** Example: PM only command.
         * This example shows how to make a PM-only command.
         * Because this is command is PM-only, this example also disables prefix requirement.
         * In this example, ErrorMessage is also set to null - doing so will prevent bot responding with an error - to the user, the command simply will appear to not exist.
         * 
         * Note: if you use both [GroupOnly] and [PrivateOnly], the command will be disabled completely. This is also true if [PrivateOnly] is used with any privileges requirement from example above.
         * Note: ErrorMessage can be changed to any text to customize it. This parameter is optional - if not provided, default error message is used.
         * Note: Any command requirement - such as [RequireGroupAdmin] for example - can also specify ErrorMessage in the same manner.
         ***/
        [Command("private only")]
        [PrivateOnly(ErrorMessage = null)]
        [Prefix(PrefixRequirement.Never)]
        [Summary("This command will only work in PMs!")]
        public Task CmdPrivateOnlyAsync(CommandContext context)
            => context.ReplyTextAsync("Welcome to my PM!");


        /*** Example: Aliases.
         * Currently Wolfringo Commands System doesn't have [Alias] attribute.
         * Instead, you can add multiple [Command] and [RegexCommand] attributes to a single method.
         * Internally they'll be treated as separate commands, but on surface, they'll work like aliases.
         * 
         * This example also shows usage of [HelpCategory] with priority - this category will be last on the list, as it has the lowest priority.
         * 
         * Note: [Command] and [RegexCommand] can be freely mixed.
         * Note: Only one "alias" (and command in general) will ever be executed per command invokation.
         ***/
        [Command("alias 1")]
        [Command("alias 2")]
        [HelpCategory("Aliases", -99999)]
        public Task CmdAliasesAsync(CommandContext context, ICommandInstance instance)
        {
            if (instance is StandardCommandInstance standardInstance)
                return context.ReplyTextAsync($"Executed alias `{standardInstance.Text}`");
            else if (instance is RegexCommandInstance regexInstance)
                return context.ReplyTextAsync($"Executed alias `{regexInstance.Pattern}`");
            else
                return context.ReplyTextAsync($"Executed some alias ({instance.GetType().Name})");
        }

        /*** Example: Command timeout.
         * By default, commands will time out after a long time (1 day).
         * You can however change the timeout value using Timeout property in the attribute.
         * This works for both Standard and Regex commands.
         * 
         * When command times out, it'll throw a new exception, which Commands Service will log as an error.
         * 
         * You can also disable timeout completely by setting its value to -1.
         ***/
        [Command("timeout", Timeout = 1000)]
        public async Task CmdTimeoutAsync()
        {
            // simulate long running task
            await Task.Delay(1500);

            // oops! This will time out!
            Console.WriteLine("This will never be written to console, because long-running task takes longer than Timeout!");
        }
    }
}
