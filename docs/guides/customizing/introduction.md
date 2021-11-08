---
uid: Guides.Customizing.Intro
title: Customizing Wolfringo - Intro
---

# Customizing Wolfringo
Wolfringo is designed with customizability from ground up. It achieves it mainly thanks to heavy use of Abstractions and Dependency Injection. Constructors of both @TehGM.Wolfringo.WolfClient and @TehGM.Wolfringo.Commands.CommandsService take an [IServiceProvider](xref:System.IServiceProvider) to resolve their dependencies, and @TehGM.Wolfringo.WolfClientBuilder and @TehGM.Wolfringo.Commands.CommandsServiceBuilder have special methods for registering/overwriting services. This allows you to replace most components of Wolfringo without having to clone and recompile it yourself!

Check navigation menu for guides how to customize specific parts of Wolfringo.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/configuring-normal-bot)
#### Replacing WolfClient dependencies
To replace any of @TehGM.Wolfringo.WolfClient dependencies, use `With...` commands on the builder.
```csharp
_client = new WolfClientBuilder()
    .WithResponseTypeResolver<MyResponseTypeResolver>()
    // alternatively, factory and instance patterns are also allowed
    //.WithResponseTypeResolver(provider => new MyResponseTypeResolver())
    //.WithResponseTypeResolver(_responseTypeResolver)
    .Build();
```

#### Replacing CommandsService dependencies
To replace any of @TehGM.Wolfringo.Commands.CommandsService dependencies, use `With...` commands on the builder.

```csharp
_client = new WolfClientBuilder()
    // ... client config ...
    .WithCommands(commands =>
    {
        commands
            .WithArgumentsParser<MyArgumentsParser>()
            // alternatively, factory and instance patterns are also allowed
            //.WithArgumentsParser(provider => new MyArgumentsParser())
            //.WithArgumentsParser(precreatedCustomResolver)
    })
    .Build();
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/configuring-hosted-bot)
To replace any interface of Wolfringo, simply register it as you would register any other service. Wolfringo should automatically pick it up.

```csharp
services.AddTransient<IResponseTypeResolver, MyResponseTypeResolver>();
services.AddTransient<IArgumentsParser, MyArgumentsParser>();
```

Refer to [Dependency injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) and [Dependency injection in ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0) for more information about Dependency Injection in [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0)/[ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.0).

> [!TIP]
> Check [WolfClientServiceCollectionExtensions](https://github.com/TehGM/Wolfringo/tree/master/Wolfringo.Hosting/WolfClientServiceCollectionExtensions.cs) and [CommandsServiceCollectionExtensions](https://github.com/TehGM/Wolfringo/tree/master/Wolfringo.Hosting/Commands/CommandsServiceCollectionExtensions.cs) to check what lifetimes the services are registered with by default.

***

## IDisposable services
Your custom service can implement @System.IDisposable - both @TehGM.Wolfringo.WolfClient and @TehGM.Wolfringo.Commands.CommandsService handle disposable services depending on how they were registered.

Service will be automatically disposed for you when <xref:TehGM.Wolfringo.WolfClient>/<xref:TehGM.Wolfringo.Commands.CommandsService> is disposing service was registered by either:
- using generic method (`WithResponseTypeResolver<MyResponseTypeResolver>()`;
- using factory method (`WithResponseTypeResolver(provider => new MyResponseTypeResolver())`;
- using Wolfringo.Hosting.

If service was registered using concrete type, or you didn't use @TehGM.Wolfringo.WolfClientBuilder or @TehGM.Wolfringo.Commands.CommandsServiceBuilder and opted to use constructors directly, you'll need to dispose these services manually.  
This is because Wolfringo has no way to know whether you're using these services in other places - disposing them in such case would break your code.