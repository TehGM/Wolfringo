---
uid: Guides.Customizing.Client.ClientCache
title: Customizing Wolfringo - Client Cache
---

# Client Cache
As explained in [Caching Guide](xref:Guides.Features.Caching), @TehGM.Wolfringo.WolfClient caches WOLF entities in memory to avoid excessive network calls. The default implementation scans all sent and received messages, and alters its internal caches accordingly. [Caching Guide](xref:Guides.Features.Caching) shows how to selectively disabling caches for specific entities if needed.

You can check the default implementation on GitHub: [WolfClientCache.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Core/Caching/Internal/WolfClientCache.cs).

## Extending Existing Cache
If you wish to extend current cache (like add support for custom [Message](xref:Guides.Customizing.Client.Messages) or [Response](xref:Guides.Customizing.Client.Responses) types), but not replace the default caching logic, you can achieve this by creating a new class that inherits from @TehGM.Wolfringo.Caching.Internal.WolfClientCache - this class declares its methods for reading sent and received messages as `virtual`, so they can easily be extended. Additionally it has protected collections for cached entities, and public flags whether they're enabled:
- @TehGM.Wolfringo.Caching.Internal.WolfClientCache.UsersCache - flag @TehGM.Wolfringo.Caching.Internal.WolfClientCache.IsUsersCachingEnabled
- @TehGM.Wolfringo.Caching.Internal.WolfClientCache.GroupsCache - flag @TehGM.Wolfringo.Caching.Internal.WolfClientCache.IsGroupsCachingEnabled
- @TehGM.Wolfringo.Caching.Internal.WolfClientCache.CharmsCache - flag @TehGM.Wolfringo.Caching.Internal.WolfClientCache.IsCharmsCachingEnabled
- @TehGM.Wolfringo.Caching.Internal.WolfClientCache.AchievementsCache - flag @TehGM.Wolfringo.Caching.Internal.WolfClientCache.IsAchievementsCachingEnabled

You can use all these features to easily add new caching logic

```csharp
public class MyCustomWolfClient : WolfClientCache
{
    // override this method to handle sent message, and server's response
    public override async Task HandleMessageSentAsync(IWolfClient client, IWolfMessage message, 
        IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default)
    {
        // make sure to call base method to ensure default logic runs
        await base.HandleMessageSentAsync(client, message, response, rawResponse, cancellationToken);
        
        if (base.IsUsersCachingEnabled && response is MyCustomResponse customResponse)
        {
            // do something
        }
    }

    // override this method to handle received events and messages
    public override async Task HandleMessageReceivedAsync(IWolfClient client, IWolfMessage message, 
        SerializedMessageData rawMessage, CancellationToken cancellationToken = default)
    {
        // make sure to call base method to ensure default logic runs
        await base.HandleMessageReceivedAsync(client, message, rawMessage, cancellationToken);
        
        if (base.IsUsersCachingEnabled && message is MyCustomMessage customMessage)
        {
            // do something
        }
    }
}
```

## Custom Client Cache
If you need more customization of entity caching (for example, you want to cache using [Redis](https://redis.io/)), you can create a new class implementing @TehGM.Wolfringo.Caching.IWolfClientCache interface. This abstraction requires a few components:
- *OnMessageSentAsync()* - invoked when @TehGM.Wolfringo.IWolfClient has sent a message and has received a server response. You can use it to cache entities sent by the server in a response. *Note: if the response indicated an error, this method will not be called*.
- *OnMessageReceivedAsync* - invoked when @TehGM.Wolfringo.IWolfClient has received a new event or message from the server. You can use it to cache entities sent by the server.
- *Clear()* - called by the client when it disconnects from WOLF server to clear all caches.
- A few methods to retrieve specific entities from the cache - see @TehGM.Wolfringo.Caching.IWolfClientCacheAccessor.

Once your custom class is finished, you need to register it with @TehGM.Wolfringo.WolfClientBuilder as explained in [Introduction](xref:Guides.Customizing.Intro).