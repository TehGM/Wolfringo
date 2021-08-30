---
uid: Guides.Customizing.Commands.CommandType
title: Customizing Wolfringo - Command Types
---

# Creating new Command Types
Wolfringo comes with [standard \[Command\]](xref:TehGM.Wolfringo.Commands.CommandAttribute) and [\[RegexCommand\]](xref:TehGM.Wolfringo.Commands.RegexCommandAttribute) command types, which should provide coverage for nearly all use cases. However, to keep true to the extensibility spirit, Wolfringo commands are abstracted enough to allow you to create your own Command Type if you wish to do so!

Creating a new Command Type requires a few steps to create a few components.

## Creating an attribute
Wolfringo recognizes a method as a command if it's marked by any attribute that inherits from @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase. @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase itself is empty, but it serves as a "marker" that a given method is a command, so you need to create a custom attribute for your custom command type.

```csharp
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class MyCustomCommandAttribute : CommandAttributeBase
{
    // any properties that you need
    // constructors probably too?
}
```

For reference, check [standard CommandAttribute.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Attributes/CommandAttribute.cs) and [RegexCommandAttribute.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Attributes/RegexCommandAttribute.cs) on GitHub.

## Creating a Command Instance
Once Wolfringo scans for command handlers and methods marked with @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase, it creates an instance class for each of found methods. These instances are kept in memory for as long as @TehGM.Wolfringo.Commands.CommandsService is loaded to prevent constant rebuilding of commands whenever a message is received. These instances are what actually handles the logic of command execution.
Wolfringo comes with @TehGM.Wolfringo.Commands.Initialization.StandardCommandInstance and @TehGM.Wolfringo.Commands.Initialization.RegexCommandInstance classes built-in - but if you're building your own Command Type, chances are that you want to create your own Instance class to customize the command execution logic.

Command Instance needs to implement @TehGM.Wolfringo.Commands.Initialization.ICommandInstance interface. This interface has 2 methods, both of which need to be implemented.  
> [!TIP]
> For reference, you can view source code of both [StandardCommandInstance.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/Instances/StandardCommandInstance.cs) and [RegexCommandInstance.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/Instances/RegexCommandInstance.cs) on GitHub.

#### CheckMatchAsync
[CheckMatchAsync](xref:TehGM.Wolfringo.Commands.Initialization.ICommandInstance.CheckMatchAsync(TehGM.Wolfringo.Commands.ICommandContext,System.IServiceProvider,System.Threading.CancellationToken)) method parses the message text and determines whether the command matches, for example whether message text should is a command. This is the place responsible for checking things like prefixes, message text etc.

The method is provided 3 parameters:
- @TehGM.Wolfringo.Commands.ICommandContext - Most important thing for running the checks, context of the command that includes the received @TehGM.Wolfringo.Messages.IChatMessage, the @TehGM.Wolfringo.IWolfClient, and @TehGM.Wolfringo.Commands.CommandsOptions with information like prefix etc.
- @System.IServiceProvider - The services used by the Commands Service. Can be used to resolve any registered service as needed.
- @System.Threading.CancellationToken - Cancellation Token that can be used to abort the checks, if any is asynchronous.

The method needs to return an @TehGM.Wolfringo.Commands.ICommandResult. Commands Service will check its @TehGM.Wolfringo.Commands.ICommandResult.Status property to determine the course of action - if it's @TehGM.Wolfringo.Commands.Results.CommandResultStatus.Success, the command will be executed, @TehGM.Wolfringo.Commands.Results.CommandResultStatus.Skip will tell the service to skip this command and check another, while @TehGM.Wolfringo.Commands.Results.CommandResultStatus.Failure will cause service to abort processing message altogether. You most likely want to return either @TehGM.Wolfringo.Commands.Results.CommandResultStatus.Success on match or @TehGM.Wolfringo.Commands.Results.CommandResultStatus.Skip when there's no match.  
Wolfringo includes @TehGM.Wolfringo.Commands.Results.StandardCommandMatchResult and @TehGM.Wolfringo.Commands.Results.RegexCommandMatchResult classes, but you can create your own @TehGM.Wolfringo.Commands.ICommandResult if you need or want to.

> [!TIP]
> It is recommended that the parsing result is included in successful @TehGM.Wolfringo.Commands.ICommandResult, so it can be reused in [ExecuteAsync method](xref:Guides.Customizing.Commands.CommandType#executeasync).

#### ExecuteAsync
[ExecuteAsync](xref:TehGM.Wolfringo.Commands.Initialization.ICommandInstance.ExecuteAsync(TehGM.Wolfringo.Commands.ICommandContext,System.IServiceProvider,TehGM.Wolfringo.Commands.ICommandResult,System.Object,System.Threading.CancellationToken)) method deals with the execution of the command itself. As of current version of Wolfringo, it should also run @TehGM.Wolfringo.Commands.ICommandRequirement checks.  
Built-in command instances make use of @TehGM.Wolfringo.Commands.Parsing.IParameterBuilder. While this is recommended to use it in your custom command instance (especially since it can be [customized as well](xref:Guides.Customizing.Commands.ParameterBuilder)!), it is not a strict requirement if you need a different behaviour in your command instance.

The method is provided 5 parameters:
- @TehGM.Wolfringo.Commands.ICommandContext - Context of the command that includes the received @TehGM.Wolfringo.Messages.IChatMessage, the @TehGM.Wolfringo.IWolfClient, and @TehGM.Wolfringo.Commands.CommandsOptions with information like prefix etc.
- @System.IServiceProvider - The services used by the Commands Service. Can be used to resolve any registered service as needed, and used to inject services into command method.
- @TehGM.Wolfringo.Commands.ICommandResult - the result of [CheckMatchAsync method](xref:Guides.Customizing.Commands.CommandType#checkmatchasync). If it includes the parsing results, it can be reused now to save computation costs.
- @System.Object - the actual instance of a command handler to run the method in.
- @System.Threading.CancellationToken - Cancellation Token that can be used to abort the checks, if any is asynchronous.

Like [CheckMatchAsync method](xref:Guides.Customizing.Commands.CommandType#checkmatchasync), this method also needs to return @TehGM.Wolfringo.Commands.ICommandResult, which will tell Commands Service whether command execution was successful, failed or skipped. Built-in command instances check if the command method returned @System.Threading.Tasks.Task or [Task\<TResult\>](xref:System.Threading.Tasks.Task`1), and will await if so. They also check if the method returns @TehGM.Wolfringo.Commands.ICommandResult, which they can use. Both options are however optional in your custom instance.

### CommandInstanceBase
Both @TehGM.Wolfringo.Commands.Initialization.StandardCommandInstance and @TehGM.Wolfringo.Commands.Initialization.RegexCommandInstance inherit from an abstract class called @TehGM.Wolfringo.Commands.Initialization.CommandInstanceBase. Your custom command instance doesn't need to do so (implementing @TehGM.Wolfringo.Commands.Initialization.ICommandInstance interface is enough), but it contains 2 shared methods used by both built-in command types that you might find useful when implementing your own. Check [CommandInstanceBase.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/Instances/CommandInstanceBase.cs) on GitHub to determine if this base class will be useful for you or not.

#### CheckMatch
This simple method will check @TehGM.Wolfringo.Commands.ICommandContext to see if:
- the message is @TehGM.Wolfringo.Messages.ChatMessage;
- the message is a text message;
- the message is not deleted;
- the message wasn't sent by the bot itself (WOLF server sends you the messages you send to it);
- the prefix requirement according to @TehGM.Wolfringo.Commands.CommandsOptions.

If any of these checks fail, it'll return false. Otherwise it'll return true, and will provide start index of the command text (after prefix) and the case sensitivity options through `out` parameters.

#### InvokeCommandAsync
This is the shared helper for both built-in instances to actually invoke the command method. It takes @TehGM.Wolfringo.Commands.Parsing.ParameterBuilderValues, @System.IServiceProvider, the handler object, and a @System.Threading.CancellationToken as paremeters, and uses them to run @TehGM.Wolfringo.Commands.Parsing.IParameterBuilder and then invoke the command method itself.

Once the method is invoked, this method will check if it was a @System.Threading.Tasks.Task or [Task\<TResult\>](xref:System.Threading.Tasks.Task`1), and if so, it'll await it.  
It'll also check if the invoked method returned @TehGM.Wolfringo.Commands.ICommandResult - if so, it'll return it to the caller; otherwise it'll return a @TehGM.Wolfringo.Commands.Results.CommandExecutionResult with a @TehGM.Wolfringo.Commands.Results.CommandResultStatus.Success status.

## Command Initializer
Now that you have both your command attribute and instance ready, there's one more component required - custom @TehGM.Wolfringo.Commands.Initialization.ICommandInitializer.  
Because Wolfringo is completely agnostic to how your custom command works (besides the @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase and @TehGM.Wolfringo.Commands.Initialization.ICommandInstance required components), it has no knowledge how to create your command instance. For this reason it depends on a @TehGM.Wolfringo.Commands.Initialization.ICommandInitializer to create the instance - so you will need to implement your own.

@TehGM.Wolfringo.Commands.Initialization.ICommandInitializer only has one method - [InitializeCommand](xref:TehGM.Wolfringo.Commands.Initialization.ICommandInitializer.InitializeCommand(TehGM.Wolfringo.Commands.Initialization.ICommandInstanceDescriptor,TehGM.Wolfringo.Commands.CommandsOptions)) which just needs to return a new command instance. The creation part is up to you.

The method is provided with a few parameters that you can use when building your command instance:
- @TehGM.Wolfringo.Commands.Initialization.ICommandInstanceDescriptor - a descriptor of the command, which includes the @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase (which you can cast to your custom attribute type) and @System.Reflection.MethodInfo which is the reflection of the method this command will execute. Default @TehGM.Wolfringo.Commands.Initialization.CommandsLoader will always create a concrete @TehGM.Wolfringo.Commands.Initialization.CommandInstanceDescriptor, so if you don't use a custom @TehGM.Wolfringo.Commands.Initialization.ICommandsLoader, you can safely cast the descriptor for additional members (such as [CommandsHandlerAttribute](xref:TehGM.Wolfringo.Commands.CommandsHandlerAttribute)).
- @TehGM.Wolfringo.Commands.CommandsOptions - the commands options instance.

For reference, you can check [StandardCommandInitializer.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/Initializers/StandardCommandInitializer.cs) and [RegexCommandInitializer.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/Initializers/RegexCommandInitializer.cs) on GitHub.

### Registering custom CommandInitializer
If you create a custom @TehGM.Wolfringo.Commands.Initialization.ICommandInitializer, you need to let Wolfringo Commands System know about it. It is done by adding it to @TehGM.Wolfringo.Commands.Initialization.ICommandInitializerProvider, which maps specific command attribute to an instance of command initializer. You can create a custom one, but the default @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProvider instance uses @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProviderOptions with a dictionary map, so for most use cases, you won't need to do that.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/configuring-normal-bot)
Note: instructions skip logger setup. See [Logging guide](xref:Guides.Features.Logging) for details.
1. Manually create an instance of @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProviderOptions.
2. Add your initializer to @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProviderOptions.Initializers dictionary.
3. Create a new instance of @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProvider, passing your options instance via constructor.
4. Register your @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProvider instance with @Microsoft.Extensions.DependencyInjection.IServiceCollection.
5. Build your service provider and pass it to @TehGM.Wolfringo.Commands.CommandsService constructor.
```csharp
CommandInitializerProviderOptions commandInitializerProviderOptions = new CommandInitializerProviderOptions();
commandInitializerProviderOptions.Initializers[typeof(MyCustomCommandAttribute)] = new MyCustomCommandInitializer();
CommandInitializerProvider ommandInitializerProvider = new CommandInitializerProvider(commandInitializerProviderOptions);
IServiceCollection services = new ServiceCollection()
    .AddSingleton<ICommandInitializerProvider>(ommandInitializerProvider);
// add any other services as needed

_client = new WolfClient(logger);                                           // create wolf client
CommandsService commands = new CommandsService(_client, options, logger,    // initialize commands service
    services.BuildServiceProvider());                           // add Dependency Injection Service provider
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/configuring-hosted-bot)
1. Configure @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProviderOptions.
2. Add your initializer to @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProviderOptions.Initializers dictionary.
```csharp
services.Configure<CommandInitializerProviderOptions>(options =>
{
    options.Initializers[typeof(MyCustomCommandAttribute)] = new MyCustomCommandInitializer();
});
```

***

### Custom ICommandInitializerProvider
If the default @TehGM.Wolfringo.Commands.Initialization.CommandInitializerProvider doesn't meet your needs (check [source code on GitHub](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/Initializers/CommandInitializerProvider.cs)), you can create your own implementation. To do so, create a new class implementing @TehGM.Wolfringo.Commands.Initialization.ICommandInitializerProvider and register it with Commands Service as described in [Introduction](xref:Guides.Customizing.Intro).

## And... that's it!
You can now mark any of methods in your command handlers with `[MyCustomCommand]`, and Commands Service will fully support executing that method.