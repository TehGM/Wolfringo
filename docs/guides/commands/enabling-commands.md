---
uid: Guides.Commands.Intro
---

# Wolfringo Commands System
Using [AddMessageListener](xref:TehGM.Wolfringo.IWolfClient#TehGM_Wolfringo_IWolfClient_AddMessageListener_TehGM_Wolfringo_Utilities_Internal_IMessageCallback_) works well for testing, or for events that aren't [Chat Messages](xref:TehGM.Wolfringo.Messages.ChatMessage). However having a bot written using just the message listeners would mean that you manually need to handle errors logging, prefix checking, checking if user is admin, etc - that's A LOT of repetitive boilerplate code.

To address this, Wolfringo has an extensible Commands System. This System is attribute-based, and supports [Dependency Injection](xref:Guides.Commands.DependencyInjection) out of the box.

## Enable Commands System in your bot
Commands System is included in both [Wolfringo metapackage](https://www.nuget.org/packages/Wolfringo) and [Wolfringo.Hosting](https://www.nuget.org/packages/Wolfringo.Hosting). If you're not using either (for example when using [Wolfringo.Core](https://www.nuget.org/packages/Wolfringo.Core) directly), it can be installed with [Wolfringo.Commands](https://www.nuget.org/packages/Wolfringo.Commands) package. See [Installation instructions](xref:Guides.GettingStarted.Installation) for guide how to install Wolfringo components.

Like with the [Bot itself](xref:Guides.GettingStarted.Connecting), enabling Commands System depends on whether you're using Wolfringo.Hosting or not.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/connecting-normal-bot)
First add following using directive to your Program.cs:
```csharp
using Wolfringo.Commands;
```

Since Wolfringo v2.0, commands can be really easily enabled by calling `WithCommands` method on the client builder. This method expects a delegate, where you can build commands service.

```csharp
_client = new WolfClientBuilder()
    // .. other client configuration ...
    .WithLogging(CreateLoggerFactory())
    .WithCommands(commands => 
    {
        // determines what all the commands should start with - for example "!", "!mybot" etc. Default value is "!".
        commands.WithPrefix("!");
        // determines when the prefix is required. By default it's always required, but you can for example make prefix optional by setting this value to PrefixRequirement.Group.
        commands.WithPrefixRequirement(PrefixRequirement.Always);
        // determines whether commands are case-sensitive. By default, all commands are case-insensitive.
        commands.WithCaseSensitivity(false);

        // you can also register any custom service by using
        // commands.WithService<T>();
    })
    .Build();

// connect bot here with "_client.ConnectAsync();"
```

Commands enable this way will automatically inherit all services and settings, such as logging and Wolf Client itself provided to `WolfClientBuilder` itself. Services set inside of `WithCommands` delegate will be available only to Commands System.

If you wish, you can create CommandsService separately to get more control. Please note that in such case, you need to manually provide @TehGM.Wolfringo.IWolfClient instance, call `StartAsync()`, and that services and logging will NOT be automatically shared.

```csharp
_client = new WolfClientBuilder()
    .WithLogging(CreateLoggerFactory())
    .Build();
CommandsService commands = new CommandsServiceBuilder()
    .WithWolfClient(_client)            // required when creating separately
    .WithLogging(CreateLoggerFactory()) // when created separately, client and commands don't share logging
    .ConfigureOptions(options => 
    {
        // do configuration here
    })
    .Build();

await services.StartAsync();
// connect bot here with "_client.ConnectAsync();"
```

### Choose where commands are loaded from
By default, all commands in the project that starts your bot process are loaded. You can change that using `.ConfigureOptions` method and the @TehGM.Wolfringo.Commands.CommandsOptions it provides in the delegate.

#### Load commands from other assemblies
You can load commands from other projects, or even different libraries. To do so, simply add assembly to [Assemblies](xref:TehGM.Wolfringo.Commands.CommandsOptions#TehGM_Wolfringo_Commands_CommandsOptions_Assemblies) property:
```csharp
.ConfigureOptions(options =>
{
    options.Assemblies.Add(typeof(HandlerInAnotherProject).Assembly));
})
```

#### Add commands individually
You can also add individual [Handler](xref:Guides.Commands.Handlers) to be loaded. Simply add its type to [Classes](xref:TehGM.Wolfringo.Commands.CommandsOptions#TehGM_Wolfringo_Commands_CommandsOptions_Classes) property:

```csharp
.ConfigureOptions(options =>
{
    options.Classes.Add(typeof(Handler));
})
```

If you're adding handlers individually, you might want to disable behaviour of loading all commands from bot entry assembly.
```csharp
.ConfigureOptions(options =>
{
    options.Assemblies.Clear();
})
```

### Logging
When using `WolfClientBuilder.WithCommands()` to add commands, Commands Service will automatically inherit logging configuration from the client.

```csharp
_client = new WolfClientBuilder()
    .WithLogging(CreateLoggerFactory())
    .WithCommands(commands => 
    {
        // do other config. Logging will automatically be inherited.
    })
    .Build();
```

If you're creating CommandsService separately using <xref:TehGM.Wolfringo.Commands.CommandsServiceBuilder>, CommandsServiceBuilder also has `WithLogging` method which functions the same way.

Check [Logging guide](xref:Guides.Features.Logging) for more information.

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/connecting-hosted-bot)
When using Wolfringo.Hosting, enabling Commands System is done inside ConfigureServices, just like [the bot itself](xref:Guides.GettingStarted.Connecting). Simply add this code:
```csharp
services.AddWolfringoCommands()
    .SetPrefix("!")           
    .SetPrefixRequirement(PrefixRequirement.Always)
    .SetCaseSensitive(false);
```

Basic Commands configuration is pretty straightforward:
- [SetPrefix](xref:Microsoft.Extensions.DependencyInjection.CommandsServiceCollectionExtensions.SetPrefix(Microsoft.Extensions.DependencyInjection.IHostedCommandsServiceBuilder,System.String,TehGM.Wolfringo.Commands.PrefixRequirement)) determines what all the commands should start with - for example "!", "!mybot" etc. Default value is "!".
- [SetPrefixRequirement](xref:Microsoft.Extensions.DependencyInjection.CommandsServiceCollectionExtensions.SetPrefixRequirement(Microsoft.Extensions.DependencyInjection.IHostedCommandsServiceBuilder,TehGM.Wolfringo.Commands.PrefixRequirement)) determines when the prefix is required. By default it's always required, but you can for example make prefix optional by setting this value to [PrefixRequirement.Group](xref:TehGM.Wolfringo.Commands.PrefixRequirement.Group).
- [SetCaseSensitive](xref:Microsoft.Extensions.DependencyInjection.CommandsServiceCollectionExtensions.SetCaseSensitive(Microsoft.Extensions.DependencyInjection.IHostedCommandsServiceBuilder,System.Boolean)) determines whether commands are case-sensitive. By default, all commands are case-insensitive (the value is set to "false").

> [!TIP]
> Using application settings file is recommended instead of hardcoding the settings. See [appsettings.json example](https://github.com/TehGM/Wolfringo/blob/master/Examples/HostedCommandsBot/appsettings.json), and add following method call to your ConfigureServices:  
> ```csharp
>  services.Configure<CommandsOptions>(context.Configuration.GetSection("Commands"));
>  ```

### Choose where commands are loaded from
By default, all commands in the project that starts your bot process are loaded. You can change that with method calls straight after AddWolfringoCommands().

#### Load commands from other assemblies
You can load commands from other projects, or even different libraries. To do so, simply call [AddHandlers()](xref:Microsoft.Extensions.DependencyInjection.CommandsServiceCollectionExtensions.AddHandlers(Microsoft.Extensions.DependencyInjection.IHostedCommandsServiceBuilder,System.Reflection.Assembly[])):
```csharp
services.AddWolfringoCommands()
    .AddHandlers(typeof(HandlerInAnotherProject).Assembly));
```

#### Add commands individually
You can also add individual [Handler](xref:Guides.Commands.Handlers) to be loaded. Simply call [AddHandler<T>()](xref:Microsoft.Extensions.DependencyInjection.CommandsServiceCollectionExtensions.AddHandler``1(Microsoft.Extensions.DependencyInjection.IHostedCommandsServiceBuilder)):
```csharp
services.AddWolfringoCommands()
    .AddHandler<Handler>();
```

If you're adding handlers individually, you might want to disable behaviour of loading all commands from bot entry assembly.
```csharp
services.AddWolfringoCommands()
    .RemoveDefaultHandlers();
```

### Logging
One of purposes of the Commands System is to reduce amount of boilerplate code for logging etc. @TehGM.Wolfringo.Hosting.Commands.HostedCommandsHandler will automatically use logging as [configured for your Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/logging/?view=aspnetcore-3.0).

Check [Logging guide](xref:Guides.Features.Logging) for more information.
***

## Next steps
Now that you enabled Commands System in your bot, you can start adding Commands Handlers and commands themselves. See [Creating Commands guide](xref:Guides.Commands.Handlers)!

You can also check [SimpleCommandsBot Example](https://github.com/TehGM/Wolfringo/tree/master/Examples/SimpleCommandsBot) (Normal Bot) or [HostedCommandsBot Example](https://github.com/TehGM/Wolfringo/tree/master/Examples/HostedCommandsBot) (.NET Generic Host/ASP.NET Core) for full example on Wolfringo usage with Commands System. Feel free to also check [other example projects](https://github.com/TehGM/Wolfringo/tree/master/Examples)!