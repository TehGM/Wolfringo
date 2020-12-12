---
uid: Guides.Commands.Handlers
title: Creating Commands
---
# Creating Commands
## Handlers
Wolfringo Commands System works using "Handlers". Commands Handlers are classes where all command methods reside. Commands System will automatically create instances of these classes.

Commands Handlers have a few requirements:
- They **cannot** be static.
- They **cannot** be abstract.
- If they have a custom constructor, all of its parameters must be possible to resolve using [Dependency Injection](xref:Guides.Commands.DependencyInjection).
- Are marked with [\[CommandsHandler\] attribute](xref:TehGM.Wolfringo.Commands.CommandsHandlerAttribute).  
  > An exception to this rule are handlers that are added individually to [CommandsOptions.Classes](xref:TehGM.Wolfringo.Commands.CommandsOptions#TehGM_Wolfringo_Commands_CommandsOptions_Classes_).

```csharp
[CommandsHandler]
private class MyCommandsHandler
{
    // any code goes here
}
```

### Handlers lifetime
By default, all handlers are **non-persistent** (aka **transient**), and are stateless. A new instance is created just before a command is executed, and discarded as soon as the command execution finishes.

For majority of use cases, transient handlers are perfectly fine. There are 2 exceptions to this:
- Your handler needs to listen to any events - be it of @TehGM.Wolfringo.IWolfClient or any other object.
- Your handler needs to store some state in its properties or variables.

If your handler meets either of these criteria, you can mark handler as **persistent** (aka **singleton**). These handlers are created as soon as [CommandService.StartAsync()](xref:TehGM.Wolfringo.Commands.CommandsService.StartAsync(System.Threading.CancellationToken)) is called, and will be kept in memory until Commands Service is disposed or application terminates.  
To mark handler as persistent, set [IsPersistent](xref:TehGM.Wolfringo.Commands.CommandsHandlerAttribute.IsPersistent) to true:
```csharp
[CommandsHandler(IsPersistent = true)]
private class MyPersistentCommandsHandler
{
    // use any properties or event listeners here
}
```

### Disposable Handlers
Sometimes Commands Handlers use unmanaged resources or other disposable classes inside. If your handler needs to be disposed, just make it implement @System.IDisposable interface.
```csharp
[CommandsHandler]
private class MyDisposableCommandsHandler : IDisposable
{
    public void Dispose()
    {
        // your disposing logic here
    }
}
```

@TehGM.Wolfringo.Commands.CommandsService will automatically call Dispose() on these handlers. Non-persistent handlers will be disposed as soon as command execution finishes. Persistent handlers will be disposed when [CommandsService.Dispose()](xref:TehGM.Wolfringo.Commands.CommandsService.Dispose) is called, or application is exiting.

### Multiple constructors
If your handler has multiple constructors, the commands system will attempt to use the one that it can resolve the most parameters for. If it can't resolve any of the parameters, @TehGM.Wolfringo.Commands.CommandsService will log an error, and fail to execute any command from that handler.

By default, all non-static constructors in the handler will be taken into account. If you have a constructor (or multiple constructors) that you want to be used by Commands System, you can give it a [\[CommandsHandlerConstructor\] attribute](xref:TehGM.Wolfringo.Commands.CommandsHandlerConstructorAttribute). If any of the constructors has this attribute, Commands System will ignore all constructors without this attribute.

If multiple constructors are marked with [\[CommandsHandlerConstructor\] attribute](xref:TehGM.Wolfringo.Commands.CommandsHandlerConstructorAttribute), by default Commands System will try to find the constructor that it can resolve most attributes for - just like when none of the constructors have the attribute. You can override this by giving [\[CommandsHandlerConstructor\]](xref:TehGM.Wolfringo.Commands.CommandsHandlerConstructorAttribute) a priority value.
```csharp
[CommandsHandler]
private class ExampleCommandsHandler
{
    // constructor with priority of 10
    [CommandsHandlerConstructor(10)]
    public ExampleCommandsHandler(IWolfClient client)
    {
        // ...
    }
}
```

Commands System will attempt to use constructors with higher priority before constructors with lower priority. If you don't specify priority, the constructor will have default priority of 0.

## Commands methods
Commands methods are the methods that are executed when command is invoked. Commands methods:
- **Cannot** be static
- Should **not** be "async void". If you need "async" in your command, return a @System.Threading.Tasks.Task or a @System.Threading.Tasks.Task`1 instead.
- Need to be marked as a command.

### Marking method as a command
Wolfringo Commands System includes support for 2 types of commands - Standard and Regex.

Standard Commands are marked with [\[Command\] attribute](xref:TehGM.Wolfringo.Commands.CommandAttribute). They are most basic commands, and you might be accomodated with them if you used other bot libraries.  
Regex Commands are marked with [\[RegexCommand\] attribute](xref:TehGM.Wolfringo.Commands.RegexCommandAttribute). They utilize power of [Regular Expressions](https://docs.microsoft.com/en-gb/dotnet/standard/base-types/regular-expressions) to allow advanced text processing and matching.

```csharp
[Command("standard")]
private async Task StandardCommandExampleAsync()
{
    // command code
}

[RegexCommand("^regex")]
private async Task RegexCommandExampleAsync()
{
    // command code
}

```

### Command parameters
A command would be useless if it had no knowledge on what the message is, or how to reply to the user. This information is provided to commands methods by Commands System via parameters.  
Commands can have following types of parameters:

#### CommandContext
Command context represents fundamental information about the command being executed, such as the message or wolf client instance. To use the context, use @TehGM.Wolfringo.Commands.CommandContext or @TehGM.Wolfringo.Commands.ICommandContext as parameter type.
```csharp
[Command("example")]
private async Task ExampleAsync(CommandContext context)
{
    // command code here
}
```

Command context provides following properties:
- @TehGM.Wolfringo.Commands.CommandContext.Message - a @TehGM.Wolfringo.Messages.ChatMessage that triggered the command. Using this property, you can also ID of the user that sent the command, whether it was a group or private message, or ID of the recipient.
- @TehGM.Wolfringo.Commands.CommandContext.Client - a @TehGM.Wolfringo.IWolfClient that received the message. You can use this client to request profiles, send messages, or communicate with WOLF server.
- @TehGM.Wolfringo.Commands.CommandContext.Options - an instance of @TehGM.Wolfringo.Commands.CommandsOptions. This will be the same options as the ones that were configured when [Enabling Commands](xref:Guides.Commands.Intro) in your bot.

Command context also has a few extension methods that make building commands easier:
- [GetSenderAsync()](xref:TehGM.Wolfringo.Commands.CommandContextExtensions.GetSenderAsync(TehGM.Wolfringo.Commands.ICommandContext,System.Threading.CancellationToken)) - retrieves the @TehGM.Wolfringo.WolfUser that sent the command.
- [GetBotProfileAsync()](xref:TehGM.Wolfringo.Commands.CommandContextExtensions.GetBotProfileAsync(TehGM.Wolfringo.Commands.ICommandContext,System.Threading.CancellationToken)) - retrieves the profile (<xref:TehGM.Wolfringo.WolfUser>) of the client that received the message (so, profile of the bot).
- [GetRecipientAsync&lt;T&gt;()](xref:TehGM.Wolfringo.Commands.CommandContextExtensions.GetRecipientAsync``1(TehGM.Wolfringo.Commands.ICommandContext,System.Threading.CancellationToken)) - retrieves the recipient of the message.  
  If [CommandContext.Message](xref:TehGM.Wolfringo.Commands.CommandContext.Message) is a group message, `T` should be @TehGM.Wolfringo.WolfGroup.  
  If [CommandContext.Message](xref:TehGM.Wolfringo.Commands.CommandContext.Message) is a private message, `T` should @TehGM.Wolfringo.WolfUser, and will work the same as [GetBotProfileAsync()](xref:TehGM.Wolfringo.Commands.CommandContextExtensions.GetBotProfileAsync(TehGM.Wolfringo.Commands.ICommandContext,System.Threading.CancellationToken)).  
  If you provide wrong generic type, this method will return `null`.
- [ReplyTextAsync(text)](xref:TehGM.Wolfringo.Commands.CommandContextExtensions.ReplyTextAsync(TehGM.Wolfringo.Commands.ICommandContext,System.String,System.Threading.CancellationToken)) - sends a text response.
- [ReplyImageAsync(text)](xref:TehGM.Wolfringo.Commands.CommandContextExtensions.ReplyImageAsync(TehGM.Wolfringo.Commands.ICommandContext,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - sends an image response.
- [ReplyVoiceAsync(text)](xref:TehGM.Wolfringo.Commands.CommandContextExtensions.ReplyVoiceAsync(TehGM.Wolfringo.Commands.ICommandContext,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - sends a voice response.

```csharp
[Command("example")]
private async Task ExampleAsync(CommandContext context)
{
    await context.ReplyTextAsync("Test passed!");
}
```

#### IWolfClient
You can pass @TehGM.Wolfringo.IWolfClient as parameter. It'll be the same client as [CommandContext.Client](xref:TehGM.Wolfringo.Commands.CommandContext.Client) property.

#### ChatMessage
You can pass @TehGM.Wolfringo.Messages.ChatMessage as parameter. It'll be the same message as [CommandContext.Message](xref:TehGM.Wolfringo.Commands.CommandContext.Message) property.

#### Command Arguments
Any text from message after the command itself is treated as arguments. A parameter will be treated as an argument if its type isn't any of the types mentioned above, and an @TehGM.Wolfringo.Commands.Parsing.IArgumentConverter is registered in @TehGM.Wolfringo.Commands.Parsing.IArgumentConverterProvider. Types supported by default:
- @System.String
- @System.Boolean
- @System.Char
- @System.DateTime
- @System.DateTimeOffset
- @System.TimeSpan
- @System.Int16, @System.UInt16
- @System.Int32, @System.UInt32
- @System.Int64, @System.UInt64
- @System.Byte, @System.SByte
- @System.Single
- @System.Double
- @System.Decimal
- @System.Numerics.BigInteger
- @TehGM.Wolfringo.WolfTimestamp
- Any enum type

##### Arguments splitting
The way arguments are split varies between Standard and Regex commands.

In Standard commands, arguments are split by space, unless they're grouped. Commands are grouped if they're wrapped into `" "`, `[ ]` or `( )`.  
Example: assume that you have a command `[Command("test")]`, and prefix is `!`.  
If user sends "!test foo bar 2 my group", there will be 5 arguments: `foo`, `bar`, `2`, `my` and `group`.  
If user sends "!test foo (bar 2) [my group]", there will be 3 arguments: `foo`, `bar 2` and `my group`.

In Regex commands, arguments will represent value of each of [regex groups](https://docs.microsoft.com/en-gb/dotnet/standard/base-types/grouping-constructs-in-regular-expressions).

> [!TIP]
> The markers (`" "`, `[ ]` and `( )`) can be modified by changing @TehGM.Wolfringo.Commands.Parsing.ArgumentsParserOptions.  
> In normal bots, manual creation of @TehGM.Wolfringo.Commands.Parsing.ArgumentsParser and providing it to Commands Service using [Dependency Injection](xref:Guides.Commands.DependencyInjection).  
> In bots using Wolfringo.Hosting, you can use methods like [AddArgumentBlockMarker(char, char)](xref:Microsoft.Extensions.DependencyInjection.CommandsServiceCollectionExtensions.AddArgumentBlockMarker(Microsoft.Extensions.DependencyInjection.IHostedCommandsServiceBuilder,System.Char,System.Char)) or [RemoveArgumentBlockMarker(char)](xref:Microsoft.Extensions.DependencyInjection.CommandsServiceCollectionExtensions.RemoveArgumentBlockMarker(Microsoft.Extensions.DependencyInjection.IHostedCommandsServiceBuilder,System.Char)), or use [Options Pattern](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.0).

##### Arguments order
Arguments are ordered as they appear in message. They'll be attempted to be converted and inserted into the method in the same order.

Assume that you have a command `[Command("test")]`, and prefix is `!`.  
User sends "!test foo bar".
```csharp
[Command("test")]
private async Task TestAsync(string argument1, string argument2)
{
    Console.WriteLine(argument1);	// will print "foo"
    Console.WriteLine(argument2);	// will print "bar"
}
```

##### Customizing error messages
When user invokes a command and argument is missing or cannot be converted, Commands System will automatically reply with a default error message. You can customize these errors using attributes:
- [\[MissingError(text)\]](xref:TehGM.Wolfringo.Commands.MissingErrorAttribute) to customize error when user didn't provide the argument at all;
- [\[ConvertingError(text)\]](xref:TehGM.Wolfringo.Commands.ConvertingErrorAttribute) to customize error when converting of an argument failed.
```csharp
[Command("example")]
private async Task ExampleAsync(
    [ConvertingError("(n) '{{Arg}}' is not a valid number!")]
    [MissingError("(n) You need to provide delay value!")] int number)
{
    // command code here
}
```

As example above shows, these custom messages can also have placeholders inside of them. These placeholders will be automatically replaced with corresponding values:
- `{{Arg}}` - value of the argument as provided by user sending the message. Will be replaced with empty string when used with [\[MissingError(text)\] attribute](xref:TehGM.Wolfringo.Commands.MissingErrorAttribute).
- `{{Type}}` - parameter type.
- `{{Name}}` - parameter name.
- `{{Message}}` - all text of the message sent by the user when invoking the command.
- `{{SenderNickname}}` - nickname (display name) of the user that invoked the command.
- `{{SenderID}}` - ID of the user that invoked the command.
- `{{BotNickname}}` - nickname (display name) of the bot.
- `{{BotID}}` - ID of the bot.

You can also set text to `null` or empty string - in such case, error response will be disabled for that command.

> [!TIP]
> [\[ConvertingError(text)\]](xref:TehGM.Wolfringo.Commands.ConvertingErrorAttribute) will never be triggered if parameter type is @System.String, as arguments are handled as strings internally.

##### Optional arguments
To mark argument as optional, set a default value for that parameter:
```csharp
[Command("example")]
private async Task ExampleAsync(string optionalArgument = null)
{
    // command code here
}
```

Optional arguments will not cause an error if they're missing - command will still run, and parameter will simply use the default value.  
> [!WARNING]
> Optional values disable [\[MissingError(text)\]](xref:TehGM.Wolfringo.Commands.MissingErrorAttribute), but they do **not** disable [\[ConvertingError(text)\]](xref:TehGM.Wolfringo.Commands.ConvertingErrorAttribute) - bot will still reply with an error if optional argument was provided, but converting has failed.

##### Catch-all
If you use @System.String[] as a parameter type, all arguments will be inserted into it.  
> [!WARNING]
> [Argument Group](#arguments-splitting) markers will not be included, only the values themselves. If you want to grab full text of the message, use `Text` property of [CommandContext.Message](xref:TehGM.Wolfringo.Commands.CommandContext.Message).

#### CancellationToken
You can pass @System.Threading.CancellationToken as paremeter. You can then use this cancellation token in your other asynchronous calls. This cancellation token will be set to cancelled when @TehGM.Wolfringo.Commands.CommandsService is being disposed - for example when the application is exiting.

```csharp
[Command("example")]
private async Task ExampleAsync(CommandContext context, CancellationToken cancellationToken)
{
    string fileContents = await File.ReadAllTextAsync("myfile.txt", cancellationToken);
}
```

#### ILogger
If you enabled logging when you were creating @TehGM.Wolfringo.Commands.CommandsService (by passing a logger into constructor, or using .NET Generic Host/ASP.NET Core), you can pass in an instance of @Microsoft.Extensions.Logging.ILogger. You can then use that in your command code to log anything you want.

#### Dependency Injection services
Any services registered with [Dependency Injection](xref:Guides.Commands.DependencyInjection) can also be used as a parameter. Please check [Dependency Injection guide](xref:Guides.Commands.DependencyInjection) for more information.

#### ICommandInstance
Internally, all [\[Command\]](xref:TehGM.Wolfringo.Commands.CommandAttribute) and [\[RegexCommand\]](xref:TehGM.Wolfringo.Commands.RegexCommandAttribute) are converted into command instance objects. For most scenarios this just a fun-fact, but sometimes (for example, when using [Aliases](#aliases)) you may want to get access to the instance of the class. To do so, simply add a parameter of type @TehGM.Wolfringo.Commands.Initialization.ICommandInstance.

### Commands Priorities
Commands System will always execute maximum of **one** command method, even if multiple commands could be triggered by user's text. For example: `[Command("test")]` and `[Command("test2")]`.  
If you want to control which command gets attempted first, use [\[Priority(value)\] attribute](xref:TehGM.Wolfringo.Commands.PriorityAttribute). Commands with highest priority value will be checked first.

```csharp
[Command("example")]
[Priority(5)]
public async Task Example1()
{
    // command code here
}
[Command("example")]
[Priority(15)]
public async Task Example2()
{
    // command code here
}
```

In the example above, command Example1 will never be executed, because Example2 has the same text, but higher priority.

### Overriding Options
@TehGM.Wolfringo.Commands.CommandsOptions class holds default settings for commands - you can customize them when [enabling Commands System](xref:Guides.Commands.Intro). On top of that, you can also change these settings using attributes.  
These attributes can be put on the command method or on the entire handler. Commands options are checked in the following order:
1. Attributes on the method.
2. Attributes on the handler.
3. @TehGM.Wolfringo.Commands.CommandsOptions instance.

For example, in following example, Prefix for ExampleCommand1 will be `!`, but for ExampleCommand2 it will be `?`:
```csharp
[CommandsHandler]
[Prefix("!")]
private class ExampleCommandsHandler
{
    [Command("example1")]
    private void ExampleCommand1() { }

    [Command("example2")]
    [Prefix("?")]
    private void ExampleCommand2() { }
}
```

Following attributes are available for use:
- [\[Prefix(string)\]](xref:TehGM.Wolfringo.Commands.PrefixAttribute) - overrides command's prefix.
- [\[Prefix(PrefixRequirement)\]](xref:TehGM.Wolfringo.Commands.PrefixAttribute) - overrides command's prefix requirement.
- [\[Prefix(string, PrefixRequirement)\]](xref:TehGM.Wolfringo.Commands.PrefixAttribute) - overrides both command's prefix and prefix requirement.
- [\[CaseSensitivity(bool)\]](xref:TehGM.Wolfringo.Commands.CaseSensitivityAttribute) - overrides command's case sensitivity.

### Aliases
Wolfringo does not have `[Alias]` attribute. Instead, the same method can have multiple attributes. Internally they're completely separate command instances, but because Commands System only ever runs one command at once, they'll work as if they were aliases.

```csharp
[Command("alias 1")]
[Command("alias 2")]
public Task ExampleAsync()
{
    // command code
}
```

## What's next?
### Examples
Example projects include example Commands Handlers:
- Check [ExampleTransientCommandsHandler.cs](https://github.com/TehGM/Wolfringo/tree/master/Examples/SimpleCommandsBot/ExampleTransientCommandsHandler.cs) for more examples on creating commands methods.
- Check [ExamplePersistentCommandsHandler.cs](https://github.com/TehGM/Wolfringo/tree/master/Examples/SimpleCommandsBot/ExamplePersistentCommandsHandler.cs) for example on Persistent (singleton) handler.

### Moving further
Now you are able to create new commands using Wolfringo Commands System, but the system's possibilities do not end here.

If you want to check how to avoid repetitive code even further, check out [Commands Requirements guide](xref:Guides.Commands.Requirements).  
Make sure to also check out [Dependency Injection guide](xref:Guides.Commands.DependencyInjection) to learn how to let commands use other systems in your bot!
