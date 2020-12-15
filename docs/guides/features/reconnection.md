---
uid: Guides.Features.Reconnection
title: Auto-Reconnecting
---

# Auto-Reconnecting
When it comes to networking, disconnection can happen at any moment. Additionally, WOLF server force disconnects after an hour of a connection.

To address this, Wolfringo offers some built-in reconnection solutions.

## [Without Wolfringo.Hosting (Normal Bot)](#tab/reconnecting-normal-bot)
### Enabling auto-connection
Auto-reconnection without Wolfringo.Hosting needs to be enabled manually. @TehGM.Wolfringo.WolfClient does not automatically reconnect on its own.  
[Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities) (installed by [Wolfringo](https://www.nuget.org/packages/Wolfringo) metapackage automatically) includes @TehGM.Wolfringo.Utilities.WolfClientReconnector helper class that can be used for easy auto-reconnection.

@TehGM.Wolfringo.Utilities.WolfClientReconnector is a separate class to adhere to SOLID principles, and to make its reconnection logic independent on implementation of @TehGM.Wolfringo.IWolfClient. Its constructor takes a @TehGM.Wolfringo.IWolfClient that it'll handle reconnection for, and it'll listen to @TehGM.Wolfringo.IWolfClient.Disconnected event. Once disconnection happens, it'll trigger a reconnection.

To use it, create instance of @TehGM.Wolfringo.Utilities.ReconnectorConfig, and create a new instance of @TehGM.Wolfringo.Utilities.WolfClientReconnector using these options and instance of your @TehGM.Wolfringo.IWolfClient.

```csharp
_client = new WolfClient();
ReconnectorConfig options = new ReconnectorConfig();
WolfClientReconnector reconnector = new WolfClientReconnector(_client, options);
```

> [!WARNING]
> Do **NOT** use @TehGM.Wolfringo.Utilities.WolfClientReconnector with Wolfringo.Hosting!

### Configuring auto-reconnection
@TehGM.Wolfringo.Utilities.ReconnectorConfig class has a few properties which can be used to customize reconnection behaviour:
- @TehGM.Wolfringo.Utilities.ReconnectorConfig.ReconnectAttempts - number of reconnection attempts to make. If set to 0, reconnection will be disabled. If set to negative number, infinite amount of attempts will be made. *Defaults to 5*.
- @TehGM.Wolfringo.Utilities.ReconnectorConfig.ReconnectionDelay - a @System.TimeSpan that will be waited between reconnection attempts. 0 or negative values disable the wait time. *Defaults to half second*.
- @TehGM.Wolfringo.Utilities.ReconnectorConfig.Log - an @Microsoft.Extensions.Logging.ILogger to log any messages with. See [Logging guide](xref:Guides.Features.Logging) for more info. If null, logging will be disabled. *Defaults to null*.
- @TehGM.Wolfringo.Utilities.ReconnectorConfig.CancellationToken - a @System.Threading.CancellationToken that will cancel any reconnections.

### Auto-reconnection errors
If @TehGM.Wolfringo.Utilities.ReconnectorConfig.Log property of @TehGM.Wolfringo.Utilities.ReconnectorConfig is not null, all error messages will be logged using that @Microsoft.Extensions.Logging.ILogger.

If reconnector makes [ReconnectorConfig.ReconnectAttempts](xref:TehGM.Wolfringo.Utilities.ReconnectorConfig.ReconnectAttempts) to reconnect and each attempt fails, the reconnector will invoke @TehGM.Wolfringo.Utilities.WolfClientReconnector.FailedToReconnect event - you can listen to that event to handle reconnection failed in your code. @TehGM.Wolfringo.Utilities.WolfClientReconnector.FailedToReconnect event provides @System.UnhandledExceptionEventArgs, which in turn has @System.UnhandledExceptionEventArgs.ExceptionObject. This object will be set to an @System.AggregateException, which contains all exceptions that occured when trying to reconnect. See [Microsoft docs](https://docs.microsoft.com/en-gb/dotnet/api/system.aggregateexception?view=netcore-3.0) to see how to handle that exception.

> [!WARNING]
> If @TehGM.Wolfringo.Utilities.ReconnectorConfig.ReconnectAttempts are set to infinite (negative value), no error will be logged and no error will be raised!

### Disabling auto-reconnection
@TehGM.Wolfringo.Utilities.WolfClientReconnector will attempt to reconnect no matter what was the cause of disconnection. If you want to disable this behaviour, simply call @TehGM.Wolfringo.Utilities.WolfClientReconnector.Dispose(). Once disposed, reconnector will be disabled permanently - to re-enable, create a new @TehGM.Wolfringo.Utilities.WolfClientReconnector instance.

> [!TIP]
> If @TehGM.Wolfringo.Utilities.WolfClientReconnector is not good enough for your needs, you can create your own reconnection with your own logic. See [WolfClientReconnector source code](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Utilities/WolfClientReconnector.cs) if you need guidance or ideas.

## [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/reconnecting-hosted-bot)
### Enabling auto-connection
@TehGM.Wolfringo.Hosting.HostedWolfClient, which Wolfringo.Hosting wrapper for @TehGM.Wolfringo.WolfClient, includes reconnection behaviour and it's enabled by default. You don't need to perform any additional steps to turn it on.

### Configuring auto-reconnection
Reconnection in Wolfringo.Hosting can be customized by changing properties of @TehGM.Wolfringo.Hosting.HostedWolfClientOptions. You can do it either using appsettings.json, or configuration methods in *ConfigureServices*:
```csharp
services.AddWolfClient()
    .SetAutoReconnectAttempts(5)
    .SetAutoReconnectDelay(TimeSpan.FromSeconds(0.5));
```

- @TehGM.Wolfringo.Hosting.HostedWolfClientOptions.AutoReconnectAttempts - number of reconnection attempts to make. If set to 0, reconnection will be disabled. If set to negative number, infinite amount of attempts will be made. *Defaults to 5*.
- @TehGM.Wolfringo.Hosting.HostedWolfClientOptions.AutoReconnectDelay - a @System.TimeSpan that will be waited between reconnection attempts. 0 or negative values disable the wait time. *Defaults to half second*.

### Auto-reconnection errors
@TehGM.Wolfringo.Hosting.HostedWolfClient will automatically use configured [logging](xref:Guides.Features.Logging) to log each error that happens when reconnecting.

If client makes @TehGM.Wolfringo.Hosting.HostedWolfClientOptions.AutoReconnectAttempts to reconnect and each attempt fails, the client will invoke @TehGM.Wolfringo.Hosting.HostedWolfClient.ErrorRaised event. Its @System.UnhandledExceptionEventArgs contains @System.UnhandledExceptionEventArgs.ExceptionObject property, which will be set to an @System.AggregateException, which contains all exceptions that occured when trying to reconnect. See [Microsoft docs](https://docs.microsoft.com/en-gb/dotnet/api/system.aggregateexception?view=netcore-3.0) to see how to handle that exception.  
After raising the error, the client will act accordingly to [HostedWolfClientOptions.CloseOnCriticalError](xref:TehGM.Wolfringo.Hosting.HostedWolfClientOptions.CloseOnCriticalError) setting - if it's set to true, the application will be terminated; otherwise, application will keep running, but bot won't reconnect unless reconnection is triggered manually.

> [!NOTE]
> [HostedWolfClientOptions.CloseOnCriticalError](xref:TehGM.Wolfringo.Hosting.HostedWolfClientOptions.CloseOnCriticalError) setting is used by more than just reconnection attempts - it also determines how the application will behave when initial connection fails, and when bot has failed to log in automatically.

### Disabling auto-reconnection
To disable auto-reconnection, simply set @TehGM.Wolfringo.Hosting.HostedWolfClientOptions.AutoReconnectAttempts to 0. Alternatively, you can disable it in *ConfigureServices*:
```csharp
services.AddWolfClient()
    .DisableAutoReconnect();
```

> [!TIP]
> Unlike @TehGM.Wolfringo.Utilities.WolfClientReconnector, @TehGM.Wolfringo.Hosting.HostedWolfClient is aware if the disconnection was manual - if so, it will not attempt to reconnect. Because of this, you don't need to disable reconnection if you're calling [DisconnectAsync](xref:TehGM.Wolfringo.Hosting.HostedWolfClient.DisconnectAsync(System.Threading.CancellationToken)). 

***