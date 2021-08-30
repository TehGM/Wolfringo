---
uid: Guides.Customizing.Commands.CommandsLoader
title: Customizing Wolfringo - Commands Loader
---

# Commands Loader
Wolfringo's CommandsService uses @TehGM.Wolfringo.Commands.Initialization.CommandsLoader to load the commands from assembly. This default implementation will scan all classes marked with [\[CommandHandler\] attribute](xref:TehGM.Wolfringo.Commands.CommandsHandlerAttribute). Then it'll look at each type for methods marked with any attribute inheriting from @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase, and create a concrete instance of @TehGM.Wolfringo.Commands.Initialization.CommandInstanceDescriptor for each of them. Additionally it'll check that a valid @TehGM.Wolfringo.Commands.Initialization.ICommandInitializer has been registered for each command type.

You can check the default implementation on GitHub: [CommandsLoader.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Initialization/Loaders/CommandsLoader.cs).

## Custom Commands Loader
The default implementation should be sufficent as long as you only create command types as described in [Creating Command Types guide](xref:Guides.Customizing.Commands.CommandType), but if you wish to create your own loader, you can do so by creating a new class implementing @TehGM.Wolfringo.Commands.Initialization.ICommandsLoader interface.

This interface defines 3 asynchronous methods for loading commands - for loading from @System.Reflection.Assembly, from @System.Reflection.TypeInfo and finally from @System.Reflection.MethodInfo. Each of these methods needs to return an IEnumerable of [ICommandInstanceDescriptors](xref:TehGM.Wolfringo.Commands.Initialization.ICommandInstanceDescriptor).  
Command Instance Descriptor is generic cached info on the command instance for each method. It lives longer than the command instance - it's cached for the existence of @TehGM.Wolfringo.Commands.CommandsService, or until it's reloaded. These descriptors are used by [ICommandInitializers](xref:TehGM.Wolfringo.Commands.Initialization.ICommandInitializer) for creating command instances, and by @TehGM.Wolfringo.Commands.Initialization.ICommandsHandlerProvider to initialize the handler.

Once your custom class is finished, you need to register it with @TehGM.Wolfringo.Commands.CommandsService as explained in [Introduction](xref:Guides.Customizing.Intro).