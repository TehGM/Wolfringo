---
uid: Guides.Commands.DependencyInjection
title: Dependency Injection
---

# Dependency Injection in Commands System
Wolfringo is built entirely with Dependency Injection in mind, and Commands System is no exception. In fact, Commands System makes even more use of it than the rest of Wolfringo library.

Dependency Injection is a mechanism for sharing services and other dependencies with other classes by providing (injecting) these dependencies into them. Dependency Injection is the recommended way to share instances of classes (such as instance of current @TehGM.Wolfringo.IWolfClient, your database class, or any other service your bot needs to work). Using static variables/properties would work as well, but it would greatly reduce maintainability, testability and quality of the project.

In Wolfringo Commands System, Handlers constructors, commands methods, internal commands instances, and even [Commands Requirements](xref:Guides.Commands.Requirements#custom-requirements) all support injection of services via Dependency Injection.

#### Handler Constructor Dependency Injection
To inject dependencies into handler constructor, simply specify them as its parameters.
```csharp
private readonly IWolfClient _client;
private readonly IMySuperDatabase _database;

public ExampleCommandsHandler(IWolfClient client, IMySuperDatabase database)
{
    this._client = client;
    this._database = database;
}
```

If Commands System cannot resolve required dependencies for any of the available constructors, it'll log error and commands in that handler won't work. Check [Registering Services](xref:Guides.Commands.DependencyInjection#registering-services) to check how to add services to DI - for example your custom *IMySuperDatabase*.  
You can also mark services as optional by giving the parameter a default value. Commands Service will still try to resolve the dependency, but if it fails to do so, it'll simply use the default value instead of throwing errors.

#### Command Method Dependency Injection
Command methods support dependency injection in a similar manner as [Handler Constructors](xref:Guides.Commands.DependencyInjection#handler-constructor-dependency-injection) - simply specify them as the parameters.
```csharp
[Command("example")]
private async Task ExampleAsync(ICommandContext context, IWolfClient client, IMySuperDatabase database)
{
    // do something with client and database here!
}
```

If Commands System cannot resolve a required dependency of any of the parameters, it'll log an error and abort executing the command. Check [Registering Services](xref:Guides.Commands.DependencyInjection#registering-services) to check how to add services to DI - for example your custom *IMySuperDatabase*.  
You can also mark services as optional by giving the parameter a default value. Commands Service will still try to resolve the dependency, but if it fails to do so, it'll simply use the default value instead of throwing errors.

#### Command Requirement Dependency Injection
Command Requirement's [CheckAsync](xref:TehGM.Wolfringo.Commands.ICommandRequirement.CheckAsync(TehGM.Wolfringo.Commands.ICommandContext,System.IServiceProvider,System.Threading.CancellationToken)) method has @System.IServiceProvider as one of its parameters. Whenever @TehGM.Wolfringo.Commands.CommandsService runs checks, its services will be provided via this parameter. You can use it to gain access to your own services - for example your custom *IMySuperDatabase*.

See [Creating Custom Command Requirements guide](xref:Guides.Commands.Requirements#custom-requirements) for more information.

## Registering Services
Registration of Dependency Injection services varies slightly depending whether you use Wolfringo.Hosting or not.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/connecting-normal-bot)
Since Wolfringo v2.0, adding services is a matter of calling `WithSingletonService`, `WithScopedService` or `WithTransientService` on <xref:TehGM.Wolfringo.Commands.CommandsServiceBuilder>. See [Services Lifetime](#services-lifetime) to understand the difference between these methods.

```csharp
// in your MainAsync method:
_client = new WolfClientBuilder()
    .WithCommands(commands =>
    {
        commands.WithSingletonService<IMySuperDatabase, MySuperDatabase>();
    })
    .Build();
```

You can add as many custom services as you want, as long as their resolving types (in example above - *IMySuperDatabase*) are different.

> [!NOTE]
> Note: services registered with @TehGM.Wolfringo.Commands.CommandsServiceBuilder are NOT available to WolfClient, which might be important if you're [customizing Wolfringo](xref:Guides.Customizing.Intro).
> @TehGM.Wolfringo.WolfClientBuilder also has a `WithService` method. All services added with this method will be available to both WolfClient and Commands (unless they're created separately), however they will always be registered with ***Singleton*** lifetime.

> [!WARNING]
> Please note that both @TehGM.Wolfringo.Commands.CommandsServiceBuilder and @TehGM.Wolfringo.WolfClientBuilder build their @System.IServiceProvider separately. That means both will get DIFFERENT copies of the same service.  
> Note: this is not applicable to Wolfringo.Hosting - in that scenario, both WolfClient and CommandsService use the same, host-provided @System.IServiceProvider.

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/connecting-hosted-bot)
[.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0)/[ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.0) have Dependency Injection deeply baked into them, and as such, they support it out of the box.  
@TehGM.Wolfringo.Hosting.HostedWolfClient will automatically pick up any services that you register in your ConfigureServices method. You don't need to do anything else - these services will be automatically used by Wolfringo.

Refer to [Dependency injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) and [Dependency injection in ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0) for more information about Dependency Injection in [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0)/[ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.0).
***

## Services Lifetime
Services can have following lifetimes:
- Singleton (`AddSingletonService<T>()`) - this service is created once, and kept alive until application exits.
- Scoped (`AddScopedService<T>()`) - this service is created once for a received message. Each new message will get a new copy of this service.
- Scoped (`AddTransientService<T>()`) - this service is created once per injection. If the service is used in constructor, method, requirement, or any combination of them, each will receive a new copy.

It is likely that you'll mostly use singleton services.