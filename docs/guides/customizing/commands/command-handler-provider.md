---
uid: Guides.Customizing.Commands.CommandHandlerProvider
title: Customizing Wolfringo - Command Handler Provider
---

# Command Handler Provider
Wolfringo Commands Service depends on @TehGM.Wolfringo.Commands.Initialization.ICommandsHandlerProvider for creating instances of command handlers and caching persisting ones in memory.  
Wolfringo's default @TehGM.Wolfringo.Commands.Initialization.CommandsHandlerProvider will automatically attempt to find a valid Constructor (respecting [\[CommandsHandlerConstructor\] attribute](xref:TehGM.Wolfringo.Commands.CommandsHandlerConstructorAttribute)), cache @TehGM.Wolfringo.Commands.Initialization.ICommandHandlerDescriptor for future, store persistent handlers in the memory, and dispose disposable handlers when the provider itself (throught commands service) is being disposed.

You can check the default implementation on GitHub: [CommandsHandlerProvider.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/CommandsHandlerProvider.cs).

## Custom Command Handler Provider
Of for some reason the default implementation doesn't work for you, you can create a new provider. To do so, create a new class implementing @TehGM.Wolfringo.Commands.Initialization.ICommandsHandlerProvider interface.

That interface only requires one method: [GetCommandHandler](xref:TehGM.Wolfringo.Commands.Initialization.ICommandsHandlerProvider.GetCommandHandler(TehGM.Wolfringo.Commands.Initialization.ICommandInstanceDescriptor,System.IServiceProvider)). 2 parameters are provided:
- @TehGM.Wolfringo.Commands.Initialization.ICommandInstanceDescriptor - the descriptor of the command instance, contains the @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase with the attribute on the method and @System.Reflection.MethodInfo which is the reflection of the method this command will execute.
- @System.IServiceProvider - services container, that can be used for creating handlers instances with Dependency Injection.

This method is required to return an @TehGM.Wolfringo.Commands.Initialization.ICommandsHandlerProviderResult, which includes the handler descriptor (which you need to create in the method) and the actual handler instance. This will then be used by Commands Service to execute the command method.

Once your custom class is finished, you need to register it with @TehGM.Wolfringo.Commands.CommandsService as explained in [Introduction](xref:Guides.Customizing.Intro).