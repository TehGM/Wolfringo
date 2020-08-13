
# Wolfringo
[![GitHub](https://img.shields.io/github/license/TehGM/Wolfringo)](LICENSE) [![Releases](https://img.shields.io/github/v/release/TehGM/Wolfringo?include_prereleases&sort=semver)](https://github.com/TehGM/Wolfringo/releases)

This is a .NET library for WOLF (previously Palringo).

This library is designed with extensibility through Dependency Injection in mind, and is compatible with ASP.NET Core and other .NET Core Hosting scenarios through [Wolfringo.Hosting](https://github.com/TehGM/Wolfringo/packages/257845) package.

Library works with strongly-typed messages and responses, that are serialized when sending and deserialized when receiving. Message listeners can be invoked by message type, giving full benefit of strong typing. Additionally, [Wolfringo.Utilities](https://github.com/TehGM/Wolfringo/packages/257846) package provides a Sender extensions class, which abstracts common sending tasks. Utilities package is included by default with [Wolfringo](https://github.com/TehGM/Wolfringo/packages/257862) meta-package.

### Download

> This library is currently in preview and hasn't yet been battle-tested, and therefore there might be bugs and updates might introduce breaking changes, some of which might not be clearly documented. Once preview ends, I'll do my best to make the library as backwards-compatible as possible, but until 1.0.0 release, be aware of pre-release stage of this library. Any version before 0.3.0 can be especially unstable.

Preview version of this library is available as a [GitHub Package](https://github.com/TehGM/Wolfringo/packages/257862). Later versions will be available on nuget.org.

1. Create a GitHub personal access token (PAT): https://github.com/settings/tokens/new. Make sure you check `read:packages` scope.
2. Run following commands to authenticate with GitHub Packages, replacing `<GithubUsername>` and `<GithubToken>` with your github username and generated PAT, respectively:
    ```cli
    dotnet nuget add source https://nuget.pkg.github.com/TehGM/index.json -n "TehGM's GitHub" -u <GithubUsername> -p <GithubToken>
    ```
3. Install package in your project
    ```cli
    Install-Package Wolfringo -Source "TehGM's GitHub"
    ```
4. *(.NET Core Host/ASP.NET Core only)* Install Wolfringo.Hosting package
    ```cli
    Install-Package Wolfringo.Hosting -Source "TehGM's GitHub"
    ```

See [GitHub Packages](https://help.github.com/en/packages/using-github-packages-with-your-projects-ecosystem/configuring-dotnet-cli-for-use-with-github-packages#installing-a-package) for more information about installing GitHub packages.

> Note: The initial version of the package is published on nuget.org as unlisted to reserve name. However, other versions between 0.1.0 and 1.0.0 will not be published to nuget.org. For this reason, it's important to specify `-Source "TehGM's Github"` when installing the preview version.

## Usage example

```csharp
using TehGM.Wolfringo;

// create client
IWolfClient client = new WolfClient();
client.AddMessageListener<WelcomeEvent>(OnWelcome);
client.AddMessageListener<ChatMessage>(OnChatMessage);

async void OnWelcome(WelcomeEvent welcome)
{
    if (welcome.LoggedInUser == null)
    {
        // login with Sender Utility
        await client.LoginAsync("MyBotEmail", "MyBotPassword");
    }
}

async void OnChatMessage(ChatMessage message)
{
    if (message.IsText && message.StartsWith("!hello"))
    {
        // get user profile
        WolfUser user = await client.GetUserAsync(message.SenderID.Value);
        // respond to the user (in group if it's a group message, in PM if it's a private message)
        await client.ReplyTextAsync(message, $"Hello, {user.Nickname}!");
        // message someone bragging about being hello'ed!
        if (message.IsGroupMessage)
        {
            WolfGroup group = await client.GetGroupAsync(message.RecipientID);
            await client.SendPrivateTextMessageAsync(ownerUserID, $"I was greeted by {user.Nickname} in [{group.Name}] group!");
        }
        else
            await client.SendPrivateTextMessageAsync(ownerUserID, $"I was greeted by {user.Nickname} in PM!");
    }
}
```

See [Example project](Examples/SimplePingBot) for a full example.

### .NET Core Host

For use with .NET Core Host, install [Wolfringo.Hosting](https://github.com/TehGM/Wolfringo/packages/257845) package in addition to the main Wolfringo meta-package. This package contains a client wrapper suitable for use with ASP.NET Core and Generic Host.

```csharp
using TehGM.Wolfringo.Hosting;

// Configure the client options to include login credentials
services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));
// add hosted wolf client with it's default services
services.AddWolfClient();
// add message handler that implements IHostedService and takes IHostedWolfClient as one of constructor parameters
services.AddHostedService<HostedMessageHandler>();
```

See [Example project](Examples/HostedPingBot) for a full example.

## Features usage
### Receiving profile updates
WOLF protocol requires you to subscribe to Group/User profile to receive real-time updates. Wolfringo client doesn't do it automatically. This behaviour is opt-in, as it's likely not necessary for most bots.

To subscribe to profile updates, request the group/user profile or bot's contacts/groups list. Requesting profiles with [Sender Utility](Wolfringo.Utilities/Sender.cs) will automatically subscribe to updates. If you're using message classes directly, their constructors have an argument allowing you to decide if the bot should subscribe to updates.

Default [WolfClient](Wolfringo.Core/WolfClient.cs) will automatically update its caches when a profile update is received.

### Interactive
[Wolfringo.Utilities.Interactive](https://github.com/TehGM/Wolfringo/packages/261227) provides helper methods to easily await next message by user, in group, or custom conditions by providing own Func delegate.

```csharp
private async void OnChatMessage(ChatMessage message)
{
    if (!message.IsPrivateMessage) return;
    await _client.ReplyTextAsync(message, "Ready? Set? Go!");
    DateTime startTime = DateTime.UtcNow;
    ChatMessage response = await _client.AwaitNextPrivateByUserAsync(message.SenderID.Value, TimeSpan.FromSeconds(10));     // timeout is optional
    if (response == null) // if response message is null, it timed out
        await _client.ReplyTextAsync(message, "Aww, too slow. :(");
    else
    {
        double userSpeed = (DateTime.UtcNow - startTime).TotalSeconds;
        await _client.ReplyTextAsync(response, $"Congrats, you replied within {userSpeed}!");
    }
}
```
See [Example project](Examples/HostedPingBot/HostedMessageHandler.cs) for a working example.

### Auto-Reconnecting

You can use [WolfClientReconnector](Wolfringo.Utilities/WolfClientReconnector.cs) to enable configurable auto-reconnecting behaviour. This is implemented outside of [WolfClient](Wolfringo.Core/WolfClient.cs) to make reconnector implementation-independent.
```csharp
IWolfClient client = new WolfClient();
ReconnectorOptions options = new ReconnectorOptions();
// here configure options, such as attempts, delay, logger or cancellation token
WolfClientReconnector reconnector = new WolfClientReconnector(client, options);
```

The reconnector utility will make defined amount of attempts to reconnect the client. If all attempts fail, the reconnector will raise `FailedToReconnect` event and provide all exceptions occured when trying to reconnect in an `AggregateException`.

To stop reconnector, call `Dispose()`. Once disposed, the reconnector will no longer attempt to auto-reconnect, and can't be reused. If you need to re-enable reconnection, create a new instance of reconnector.

If the reconnector behaviour is not sufficent for your use-case, listen to client's Disconnected event to implement own behaviour.

> Note: do not use [WolfClientReconnector](Wolfringo.Utilities/WolfClientReconnector.cs) if using hosted client wrapper from [Wolfringo.Hosting](https://github.com/TehGM/Wolfringo/packages/257845) package. This wrapper has reconnection logic built-in.

### Logging
[WolfClient](Wolfringo.Core/WolfClient.cs) constructor takes an ILogger as one of the optional parameters, making it compatible with any of the major .NET logging frameworks. To enable logging, create a new instance of any ILogger, and simply pass it to the client constructor.

In .NET Core Host, simply configure logging using services as you normally would in ASP.NET Core/other Hosted scenario. Default [HostedWolfClient](Wolfringo.Hosting/HostedWolfClient.cs) will use dependency injection mechanisms to get the logger and pass it to the underlying [WolfClient](Wolfringo.Core/WolfClient.cs).

### Errors handling
If server responds with an error, [MessageSendingException](Wolfringo.Core/MessageSendingException.cs) will be thrown and provide a error details. To handle errors, use try-catch block when sending any message. This exception will not be logged automatically by the client.

For other errors (such as exceptions thrown when processing a received message), subscribe to [IWolfClient.ErrorRaised](Wolfringo.Core/IWolfClient.cs) event. Its `UnhandledExceptionEventArgs` contains `ExceptionObject` property, which is the that exception occured.

### Caching
Default [WolfClient](Wolfringo.Core/WolfClient.cs) automatically caches following WOLF entities: Users, Groups, Charms and Achievements. [Sender Utility](Wolfringo.Utilities/Sender.cs) automatically uses cache where possible to avoid excessive requests to the server.

Cached entities have lifetime of current connection, and will be automatically removed when client disconnects, regardless if it was a manual disconnection, or automatic hourly disconnection requested by the server.

You can selectively opt out of caching by using following properties of the client: `UsersCachingEnabled`, `GroupsCachingEnabled`, `CharmsCachingEnabled`, `AchievementsCachingEnabled`. [Hosted WolfClient](Wolfringo.Hosting/HostedWolfClientOptions) can set these properties in its appsettings section.

## Extending the client
#### Serializer maps
Client uses power of Dependency Injection to allow customizability. The client accepts optional Message and Response Serializer maps which are used for serializing and deserializing the message and response objects. You can inject own instance of the map to change mapping, or even add new types if it's required.

You can see [DefaultMessageSerializerMap](Wolfringo.Core/Messages/Serialization/DefaultMessageSerializerMap.cs), [DefaultResponseSerializerMap](Wolfringo.Core/Messages/Serialization/DefaultResponseSerializerMap.cs), [DefaultMessageSerializer](Wolfringo.Core/Messages/Serialization/Serializers/DefaultMessageSerializer.cs) and [DefaultResponseSerializer](Wolfringo.Core/Messages/Serialization/Serializers/DefaultResponseSerializer.cs) for examples of default base implementations.

#### Overriding client methods
Client automatically caches the entities based on message/response type. If you add a new type that needs to support this, you must create a new client class inheriting from [WolfClient](Wolfringo.Core/WolfClient.cs). You can override `OnMessageSentInternalAsync` method to change behaviour for sent messages and received responses, and `OnMessageReceivedInternalAsync` method to change behaviour for received events and messages.

> Note: it's recommended to still call base method after own implementation to prevent accidentally breaking default behaviour. Overriding these methods should be handled with caution.

#### Determining response type for sent message
[WolfClient](Wolfringo.Core/WolfClient.cs) needs to know how to deserialize message's response, and to determine the type, it uses an [IResponseTypeResolver](Wolfringo.Core/Messages/Responses/IResponseTypeResolver.cs) to select the type that will be used with response serializer mappings. This interface can be passed into the client constructor. If null or none is passed in, [DefaultResponseTypeResolver](Wolfringo.Core/Messages/Responses/DefaultResponseTypeResolver.cs) will be used automatically.

[DefaultResponseTypeResolver](Wolfringo.Core/Messages/Responses/DefaultResponseTypeResolver.cs) respects [ResponseType](Wolfringo.Core/Messages/Responses/ResponseTypeAttribute.cs) attribute on the message type, and will ignore the generic type passed in with `SendAsync` method. If the attribute is missing, default client implementation will instruct the type resolver to use provided generic type. Client will attempt to cast the response to the provided generic type regardless of the actual response type, and might throw an exception if the cast is impossible.

## Further development
> This library is still in preview, so breaking changes might be introduced with any version until 1.0.0 release.

#### Planned features
- Some kind of commands system.

#### Known bugs and missing features
- Avatar setting is not supported, due to Wolf protocol not supporting it yet.
- Spam filter settings is not supported.

### Contributing
In case you want to report a bug or request a feature, open a new [Issue](https://github.com/TehGM/Wolfringo/issues).

If you want to contribute a patch or update, fork repository, implement the change, and open a pull request.

## License
Copyright (c) 2020 TehGM 

Licensed under [MIT License](LICENSE).
