---
uid: Guides.Features.Caching
title: Caching
---

# Caching of WOLF Entities
WOLF server really doesn't like if you request entities from it a lot - it might even temporarily suspend your bot's accounts if you spam it too much.  
And to be honest - even if it didn't mind, requesting data from it introduces network overhead.

To avoid these issues, default @TehGM.Wolfringo.WolfClient implementation automatically caches following WOLF entities: @TehGM.Wolfringo.WolfUser, @TehGM.Wolfringo.WolfGroup, @TehGM.Wolfringo.WolfCharm and @TehGM.Wolfringo.WolfAchievement. It will also handle updating the cache when server sends [update events](xref:Guides.Features.ReceivingProfileUpdates).

Cached entities have lifetime of current connection. That means, as soon as you disconnect, all caches will automatically be cleaned. This happens regardless of the disconnection reason - caches will be purged when you disconnect manually, or when WOLF server disconnects you after an hour.

## Accessing caches
Both @TehGM.Wolfringo.WolfClient and @TehGM.Wolfringo.Hosting.HostedWolfClient hide access to their caches by default. They can however be accessed in two ways:

#### Using Sender utility
All extension methods provided by [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities), such as [GetUserAsync](xref:TehGM.Wolfringo.Sender.GetUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Threading.CancellationToken)) or [GetGroupAsync](xref:TehGM.Wolfringo.Sender.GetGroupAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Threading.CancellationToken)), will automatically retrieve entities from cache. Whenever you request an entity and it's already cached, the cached version will be used, and no request to the server will be made.

#### Casting to IWolfClientCacheAccessor
@TehGM.Wolfringo.Utilities.Internal.IWolfClientCacheAccessor is an interface that both @TehGM.Wolfringo.WolfClient and @TehGM.Wolfringo.Hosting.HostedWolfClient implement. This interface provides access to the cached entities.

You can simply cast the client to @TehGM.Wolfringo.Utilities.Internal.IWolfClientCacheAccessor and then use any of the interface's methods.
```csharp
IWolfClientCacheAccessor cacheAccessor = (IWolfClientCacheAccessor)_client;
WolfUser cachedUser = cacheAccessor.GetCachedUser(1234);
```

## Disabling cache
Wolfringo's default clients allow you to disable caches individually if you wish to do so.

> [!WARNING]
> While Wolfringo does give an option to disable caching, it is still recommended to keep them enabled to avoid issues mentioned at the beginning of this guide.

#### Without Wolfringo.Hosting
In normal bot, you can simply set any of the following properties to false: @TehGM.Wolfringo.WolfClient.UsersCachingEnabled, @TehGM.Wolfringo.WolfClient.GroupsCachingEnabled, @TehGM.Wolfringo.WolfClient.CharmsCachingEnabled and @TehGM.Wolfringo.WolfClient.AchievementsCachingEnabled.

```csharp
_client = new WolfClient();
_client.UsersCachingEnabled = false;			// disable users caching
_client.GroupsCachingEnabled = false;			// disable groups caching
_client.CharmsCachingEnabled = false;			// disable charms caching
_client.AchievementsCachingEnabled = false;		// disable achievements caching
```

#### With Wolfringo.Hosting
In a bot using Wolfringo.Hosting, you can disable caches using @TehGM.Wolfringo.Hosting.HostedWolfClientOptions. You can do this in 2 ways.

1. Configure the client in *ConfigureServices*:  
	```csharp
	services.AddWolfClient(options =>
	{
	    options.UsersCachingEnabled = false;		// disable users caching
	    options.GroupsCachingEnabled = false;		// disable groups caching
	    options.CharmsCachingEnabled = false;		// disable charms caching
	    options.AchievementsCachingEnabled = false;	// disable achievements caching
	});
	```
2. Update your appsettings.json:  
	ConfigureServices:  
    ```csharp
	services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));
	```  
	appsettings.json:
	```json
	"WolfClient": {
	  "UsersCachingEnabled": false,
	  "GroupsCachingEnabled": false,
	  "CharmsCachingEnabled": false,
	  "AchievementsCachingEnabled": false
	}
	```
