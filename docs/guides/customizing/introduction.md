---
uid: Guides.Customizing.Intro
title: Customizing Wolfringo - Intro
---

# Customizing Wolfringo
Wolfringo is designed with customizability from ground up. It achieves it mainly thanks to heavy use of Abstractions and Dependency Injection. Constructors of both @TehGM.Wolfringo.WolfClient (directly) and @TehGM.Wolfringo.Commands.CommandsService (through [IServiceProvider](xref:System.IServiceProvider)) take their dependencies as interfaces, and work with them as-is. This allows you to replace most components of Wolfringo without having to clone and recompile it yourself!

Check navigation menu for guides how to customize specific parts of Wolfringo.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/connecting-normal-bot)
#### Replacing WolfClient dependencies
To replace any of @TehGM.Wolfringo.WolfClient dependency, pass it in the constructor. 
```csharp
IResponseTypeResolver customResolver = // ...
_client = new WolfClient(log, responseTypeResolver: customResolver);
```

#### Replacing CommandsService dependencies
@TehGM.Wolfringo.Commands.CommandsService uses @System.IServiceProvider to resolve its dependencies. To replace any of the services, simply register it before creating the Commands Service.  
This works in a very similar way as [enabling logging](xref:Guides.Features.Logging).

```csharp
private static async Task MainAsync(string[] args)
{
    // ... other code ...

    ILoggerFactory logFactory = // ...                                  // create the logger factory
    IArgumentsParser customArgsParser = // ...                          // create your custom service
    IServiceCollection services = new ServiceCollection()               // create service collection
        .AddSingleton<ILoggerFactory>(logFactory)                       // include the logger factory
        .AddSingleton<IArgumentsParser(customArgsParser);               // include the custom service
    // add any other services as needed

    _client = new WolfClient(logFactory.CreateLogger<WolfClient>());    // create wolf client - pass logger via constructor
    CommandsService commands = new CommandsService(_client, options,    // initialize commands service
        logFactory.CreateLogger<CommandsService>(),                 // pass logger via constructor
        services.BuildServiceProvider());                           // add Dependency Injection Service provider

    // ... other code ...
}
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/connecting-hosted-bot)
To replace any interface of Wolfringo, simply register it as you would register any other service. Wolfringo should automatically pick it up.

Refer to [Dependency injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) and [Dependency injection in ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0) for more information about Dependency Injection in [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0)/[ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.0).