---
uid: Guides.Features.Logging
title: Logging
---

# Logging in Wolfringo
A bot is a background process - most often it'll be running in background, without user interaction. For that reason, it's important to **log** information.

Wolfringo on its own doesn't have a logger. It also doesn't have "Log" event or anything like that - these events often are error-prone, and require additional work when setting up.  
Instead Wolfringo has full support for @Microsoft.Extensions.Logging.ILogger interface. This makes it fully compatible with any relevant .NET logging library - [Serilog](https://serilog.net/), [NLog](https://nlog-project.org/), [Log4Net](https://logging.apache.org/log4net/), you name it! (Well, as long as it works with [Microsoft.Extensions.Logging.ILogger](xref:Microsoft.Extensions.Logging.ILogger)).

## Setting logging up
The exact way to set up logging might vary between libraries. Here I show a few examples.

## [Without Wolfringo.Hosting (Normal Bot)](#tab/logging-normal-bot)

All examples will use a special method called "*CreateLogger*" in Program.cs. It'll be called before creating @TehGM.Wolfringo.IWolfClient and @TehGM.Wolfringo.Commands.ICommandsService:

```csharp
private static async Task MainAsync(string[] args)
{
    // ... other code ...

    ILogger log = CreateLogger();   // create the logger
    _client = new WolfClient(log);  // create wolf client - pass logger via constructor
    CommandsService commands = new CommandsService(_client, options, log: log);     // create commands service - pass logger via constructor

    // ... other code ...
}

private static ILogger CreateLogger()
{
    // creating a logger depends on the library - see below!
}
```

> Note: You can also create separate loggers for @TehGM.Wolfringo.IWolfClient and @TehGM.Wolfringo.Commands.ICommandsService - this way, your logging will put both in separate log categories.  
> This is actually recommended - examples below don't do it just for simplicity.

### Serilog
[Serilog](https://serilog.net/) is one of the most popular (if not *the* most popular) logging libraries for .NET. It is my personal choice in my projects, so let's start with it!

First, using your NuGet package manager, install [Serilog](https://www.nuget.org/packages/serilog), [Serilog.Extensions.Logging](https://www.nuget.org/packages/Serilog.Extensions.Logging), and any of the [sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks) you want to use. For the sake of example, I will use [Serilog.Sinks.File](https://www.nuget.org/packages/Serilog.Sinks.File) and [Serilog.Sinks.Console](https://www.nuget.org/packages/Serilog.Sinks.Console).  
![](/_images/guides/logging-serilog-1.png)

Once downloaded, add following *using* directives to your Program.cs:
```csharp
using Microsoft.Extensions.Logging;
using Serilog;
```

Finally, populate your *CreateLogger* method:
```csharp
private static ILogger CreateLogger()
{
    Log.Logger = new LoggerConfiguration()  // initialize Serilog configuration
        .MinimumLevel.Information()         // set minimum logs level to Information - feel free to change it to whatever suits your needs
        .Enrich.FromLogContext()            // add log context to logs - optional, but Wolfringo makes use of it!
        .WriteTo.Console()                  // enable logging to console
        .WriteTo.File("logs.txt")           // enable logging to "logs.txt" files
        .CreateLogger();                    // create logger instance

    return new LoggerFactory()              // create factory for Microsoft.Extensions.Logging.ILogger
        .AddSerilog(Log.Logger)             // add our Serilog logger
        .CreateLogger<IWolfClient>();       // create and return our logger
}
```

Check [Serilog repo on GitHub](https://github.com/serilog/serilog) for more guides how to configure Serilog.

> Note: remember to add logger instance to @TehGM.Wolfringo.IWolfClient and @TehGM.Wolfringo.Commands.ICommandsService via constructor - if you only set *Log.Logger*, Wolfringo will not be able to log anything!

### Microsoft.Extensions.Logging
Microsoft extensions logging also has some providers. These tend to be quite basic, and using logging libraries such as [Serilog](https://serilog.net/), [NLog](https://nlog-project.org/) or [Log4Net](https://logging.apache.org/log4net/) is recommended - but it is enough to get most basic logs!

First, using your NuGet package manager, install [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/microsoft.extensions.logging.console).  
![](/_images/guides/logging-microsoft-1.png)

Once downloaded, add following *using* directive to your Program.cs:
```csharp
using Microsoft.Extensions.Logging;
```

Finally, populate your *CreateLogger* method:
```csharp
private static ILogger CreateLogger()
{
    ILoggerFactory loggerFactory = new LoggerFactory(                   // create Microsoft.Extensions.Logging.ILogger factory
            new[] { new ConsoleLoggerProvider((_, level)                // enable logging to console
              => level != LogLevel.Trace && level != LogLevel.Debug,    // disable any logs below Information - feel free to change it to whatever suits your needs
            true) }                                                     // do include log scopes
        );
    return loggerFactory.CreateLogger<T>();     // create and return our logger
}
```

### Other libraries
Many other libraries support Microsoft.Extensions.Logging. Some might support it out of the box, while some might have a wrapper package available (like Serilog). Check out their documentations to see how to use them!

> [!TIP]
> If you're searching on @Microsoft.Extensions.Logging.ILogger support for your library, you can use a phrase like "ASP.NET Core" - it includes native support for *Microsoft.Extensions.Logging*, so many articles focus on this use-case.

## [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/logging-hosted-bot)
Logging is built into [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0)/[ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.0), so setting it up is really easy - just set it as you normally would in ASP.NET Core application! 

Wolfringo.Hosting's wrappers for Wolfringo services (@TehGM.Wolfringo.Hosting.HostedWolfClient and <xref:TehGM.Wolfringo.Hosting.Commands.HostedCommandsService>) will automatically use logging if it's configured in your host.

> Note: In addition to steps I include as examples below, I recommend checking [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/logging/?view=aspnetcore-3.0) for more details!

### Serilog
[Serilog](https://serilog.net/) is one of the most popular (if not *the* most popular) logging libraries for .NET. It is my personal choice in my projects, so let's start with it!

Serilog has 2 recommended utility packages for hosted scenarios:
- [Serilog.AspNetCore](https://www.nuget.org/packages/Serilog.AspNetCore) when using ASP.NET Core
- [Serilog.Extensions.Hosting](https://www.nuget.org/packages/Serilog.Extensions.Hosting) when using Generic Host without ASP.NET Core

You will need to install the suitable utility package. To use configuration, we'll also use [Serilog.Settings.Configuration](https://www.nuget.org/packages/Serilog.Settings.Configuration). In addition, you'll want to install some [sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks) - for the sake of example, I will use [Serilog.Sinks.File](https://www.nuget.org/packages/Serilog.Sinks.File) and [Serilog.Sinks.Console](https://www.nuget.org/packages/Serilog.Sinks.Console).  
![](/_images/guides/logging-serilog-2.png)

2nd step is to enable logging in your Program.cs, where you configure your host:
```csharp
// ...
IHost host = Host.CreateDefaultBuilder(args)
    // ... other ConfigureSomething methods - such as ConfigureServices, etc
    .UseSerilog((context, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration);   // read configuration from appsettings.json
    }, true)
    .Build();
```

Lastly, you need to configure your appsettings.json to include logging configuration. Remove "Logging" section, and add a new "Serilog" section. Make sure to include every sink you're using in "Using" array, and configure each in "WriteTo" array:
```json
"Serilog": {
  "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "System.Net.Http.HttpClient": "Warning"
    }
  },
  "Enrich": [
    "FromLogContext"
  ],
  "WriteTo": [
    {
      "Name": "Console"
    },
    {
      "Name": "File",
      "Args": {
        "path": "logs.txt"
      }
    }
  ]
}
```

See [Serilog.Settings.Configuration repo on GitHub](https://github.com/serilog/serilog-settings-configuration) for more information on how to use configuration with Serilog.  
Check [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) and [Serilog.Extensions.Hosting](https://github.com/serilog/serilog-extensions-hosting) GitHub repositories as well, for more information on how to use these packages!

### Microsoft.Extensions.Logging
Microsoft extensions logging also has some providers. These tend to be quite basic, and using logging libraries such as [Serilog](https://serilog.net/), [NLog](https://nlog-project.org/) or [Log4Net](https://logging.apache.org/log4net/) is recommended - but it is enough to get most basic logs!

Console logger for this approach is already built into Hosting, so you don't need to install any packages. Simply add a new *using* directive to Program.cs:
```csharp
using Microsoft.Extensions.Logging;
```

Now, just add your configuration. In Hosted approach, logging is configured in Program.cs, where you configure your host:
```csharp
// ...
IHost host = Host.CreateDefaultBuilder(args)
    // ... other ConfigureSomething methods - such as ConfigureServices, etc
    .ConfigureLogging((context, config) =>
    {
        config.SetMinimumLevel(LogLevel.Information);   // set minimum logs level to Information - feel free to change it to whatever suits your needs
    })
    .Build();
```

### Other libraries
Many other libraries support Microsoft.Extensions.Logging. Some might support it out of the box, while some might have a wrapper package available (like Serilog). Check out their documentations to see how to use them!

> [!TIP]
> If you're searching on @Microsoft.Extensions.Logging.ILogger support for your library, you can use a phrase like "ASP.NET Core" - it includes native support for *Microsoft.Extensions.Logging*, so many articles focus on this use-case.
***

## Logging in Handlers and Services
Any service registered with Dependency Injection and all Handlers that @TehGM.Wolfringo.Commands.CommandsService loads can use @Microsoft.Extensions.Logging.ILogger - simply inject it via constructor or method parameters. Check [Dependency Injection guide](xref:Guides.Commands.DependencyInjection) for more details.

```csharp
[CommandsHandler]
private class ExampleCommandsHandler
{
    private readonly ILogger _log;

    public ExampleCommandsHandler(ILogger log)
    {
        this._log = log;
    }

    [Command("example")]
    public async Task ExampleAsync(CommandContext context, ILogger log)
    {
        log.LogInformation("Received a message from {SenderID}", context.Message.SenderID.Value);
        // can also use _log instead of log
    }
}
```

> Note: When using Wolfringo.Hosting approach, you can also inject @Microsoft.Extensions.Logging.ILogger`1 if you want your logs categorized. This is currently unavailable in "normal" bots.