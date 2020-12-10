---
uid: Guides.GettingStarted.Connecting
---

# Connecting the bot
Once Wolfringo is [installed](xref:Guides.GettingStarted.Installation), it's time to get your bot connected. The way to do it varies whether you're using Wolfringo.Hosting package or not.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/connecting-normal-bot)
First, add following using directives to your Program.cs:
```csharp
using TehGM.Wolfringo;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Utilities;
```

### Add event listeners
There are numerous events that can come from WolfClient, but the most important are when server sends "Welcome" (which is the place to login the bot) and when bot receives a chat message. To handle them, let's add 2 new event listener methods to Program class.

```csharp
// this method will be called when "Server" welcomes the bot - this is where we login!
private static async void OnWelcome(WelcomeEvent message)
{
    // if reusing the token, user might be already logged in, so check that before requesting login
    if (message.LoggedInUser == null)
    {
        // note: it is recommended to not hardcode username and password, and use .gitignore-d config file instead
        // see exaple project linked below for a full example!
        await _client.LoginAsync("BotEmail", "BotPassword", WolfLoginType.Email);
    }
    await _client.SubscribeAllMessagesAsync();      // without this, bot will not receive any messages
}

// this method will be called whenever the bot receives a message in chat
// see Commands System guides to check how implement proper commands, without using this listener at all
private static async void OnChatMessage(ChatMessage message)
{
    // reply only to private text messages that start with "!mybot hello"
    if (message.IsText && message.Text.StartsWith("!mybot hello", StringComparison.OrdinalIgnoreCase))
    {
        await _client.ReplyTextAsync(message, "Hello there!!!");
    }
}
```

### Connect the bot
With event listener methods ready, it's time to make our bot actually connect. Inside of Program class, add a new variable to store your bot client in:
```csharp
class Program
{
    private static IWolfClient _client;
}
```

Modify your Main method and add a new MainAsync method:
> Note: this step can be skipped if you're using C# 7.1 or later - in such case, simply [change return type of Main from `void` to `Task`](https://docs.microsoft.com/en-gb/dotnet/csharp/whats-new/csharp-7#async-main).
```csharp
static void Main(string[] args)
{
    MainAsync(args).GetAwaiter().GetResult();
}

static async Task MainAsync(string[] args)
{
    // other startup code will go here!
}

```

Now we need to do a few things - create a WolfClient instance, register event listeners, connect the bot, and prevent application from exiting. To do this, you can use following code inside of your MainAsync method:
```csharp
// create client and listen to events we're interested in
_client = new WolfClient(log);
_client.AddMessageListener<WelcomeEvent>(OnWelcome);
_client.AddMessageListener<ChatMessage>(OnChatMessage);

// start connection and prevent the application from closing
await _client.ConnectAsync();
await Task.Delay(-1);
```

AddMessageListener is a method to add event listener for receiving messages and events. It takes a generic parameter and a callback as normal parameter. It'll call callback when a received message is of type specified by generic parameter - for example, in the code above, OnWelcome will be called when @TehGM.Wolfringo.Messages.WelcomeEvent is received, and OnChatMessage when @TehGM.Wolfringo.Messages.ChatMessage is received.  
Wolfringo contains many message types - check @TehGM.Wolfringo.Messages to check out others!

### Make the bot reconnect automatically
We already have all code to get bot working, but it won't reconnect automatically - and WOLF protocol forces disconnection eveyr hour. Not good!  
But don't worry - there's an easy way to enable automatic reconnection.

[Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities) package (installed by default when you install [Wolfringo](https://www.nuget.org/packages/Wolfringo) metapackage) has an utility class called @TehGM.Wolfringo.Utilities.WolfClientReconnector. This class is 'outside' of WolfClient because that makes this class implementation independent.

To enable @TehGM.Wolfringo.Utilities.WolfClientReconnector, simply add this code to your MainAsync - just after you create WolfClient, but before you ConnectAsync() it:
```csharp
ReconnectorConfig reconnectorConfig = new ReconnectorConfig();
reconnectorConfig.ReconnectAttempts = -1;       // by default, ReconnectorConfig will retry 5 times - here we change it to -1, which makes it infinite
WolfClientReconnector reconnector = new WolfClientReconnector(_client, reconnectorConfig);
```



### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/connecting-hosted-bot)
### Add event listeners
To handle WolfClient events, we need a handler class. Let's create add a new class called `HostedMessageHandler` to our project. Once the class is created, add following using directives:
```csharp
using TehGM.Wolfringo;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Utilities;
```

There are numerous events that can come from WolfClient, but the most important are when server sends "Welcome" (which is the place to login the bot) and when bot receives a chat message. Handling Welcome is handled by @TehGM.Wolfringo.Hosting.HostedWolfClient automatically by default, but we still need to handle received message to determine what to do. To do so, let's add a simple event listener to our `HostedMessageHandler`:
```csharp
// this method will be called whenever the bot receives a message in chat
// see Commands System guides to check how implement proper commands, without using this listener at all
private async void OnChatMessage(ChatMessage message)
{
    // reply only to private text messages that start with "!mybot hello"
    if (message.IsText && message.Text.StartsWith("!mybot hello", StringComparison.OrdinalIgnoreCase))
    {
        await _client.ReplyTextAsync(message, "Hello there!!!");
    }
}
```

Now we need to grab our client instance, and register the listener. To do it in .NET Generic Host, we'll use [Constructor Dependency Injection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection). Add following variable and constructor for your `HostedMessageHandler`:
```csharp
private readonly IWolfClient _client;

// can also be IWolfClient
public HostedMessageHandler(IWolfClient client)
{
    this._client = client;
    this._client.AddMessageListener<ChatMessage>(OnChatMessage);
}
```
AddMessageListener is a method to add event listener for receiving messages and events. It takes a generic parameter and a callback as normal parameter. It'll call callback when a received message is of type specified by generic parameter - for example, in the code above, OnChatMessage when @TehGM.Wolfringo.Messages.ChatMessage is received.  
Wolfringo contains many message types - check @TehGM.Wolfringo.Messages to check out others!

With this in place, IDE should stop complaining, and class should be functional. However, it's never started. To get it to start on application startup, we need to implement @Microsoft.Extensions.Hosting.IHostedService:
```csharp
public class HostedMessageHandler : IHostedService
{
    // .. 
    // ... all other code here ...
    // ..

    // Implementing IHostedService ensures this class is created on start
    Task IHostedService.StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
    Task IHostedService.StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
```

### Connect the bot
Our Message Handler class is ready, now we can update our ConfigureServices method. The way it's done depends on whether you created an ASP.NET Core project or not.

#### ASP.NET Core - with Startup.cs
If your project used some of ASP.NET Core templates, most likely it'll already have a generated Startup.cs file. Let's add following using directives on top of Startup.cs:
```csharp
using Microsoft.Extensions.DependencyInjection;
using TehGM.Wolfringo.Hosting;
```

Now just add following code to `ConfigureServices` method that should already exist in your Startup.cs:
```csharp
// load configuration
services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));

// add client
// note: it is recommended to not hardcode username and password, and use .gitignore-d config file instead
// see exaple project linked below for a full example!
services.AddWolfClient()
    .SetCredentials("BotEmail", "BotPassword", WolfLoginType.Email);

// add our HostedMessageHandler
services.AddHostedService<HostedMessageHandler>();
```

#### .NET Generic Host - without Startup.cs
If your project didn't generate Startup.cs, you most likely are not making a web application using ASP.NET Core - but don't worry, that's okay, you can still use Wolfringo.Hosting!

First, install [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) NuGet package in your project. Once it's installed, add following using directives to your Program.cs:
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Hosting;
```

Now, we need to create, configure and start the host. To do so, add this to your Main method:
```csharp
// this is an example showing configuration for .NET Core 3.0+
// for host configuration instructions in .NET Core 2.1 and 2.2, 
// see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // load configuration
        services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));
        services.AddWolfClient()
            // add client
            // note: it is recommended to not hardcode username and password, and use .gitignore-d config file instead
            // see exaple project linked below for a full example!
            .SetCredentials("BotEmail", "BotPassword", WolfLoginType.Email);
            
        // add our HostedMessageHandler
        services.AddHostedService<HostedMessageHandler>();
    })
    .ConfigureLogging((context, config) =>
    {
        // configure logging here
        // see your logging library configuration instructions
        config.SetMinimumLevel(LogLevel.Debug);
    })
    .Build();
// run the host
host.RunAsync().GetAwaiter().GetResult();
```

### Make the bot reconnect automatically
Unlike bots without .NET Generic Host, @TehGM.Wolfringo.Hosting.HostedWolfClient automatically handles reconnection internally. By default it'll attempt to reconnect 5 times. You can change that by either changing "AutoReconnectAttempts" in [settings](https://github.com/TehGM/Wolfringo/blob/master/Examples/HostedPingBot/appsettings.json), or using following method:
```csharp highlight="3"
services.AddWolfClient()
    .SetCredentials("BotEmail", "BotPassword", WolfLoginType.Email)
    .SetAutoReconnectAttempts(-1);  // by default, bot will try to reconnect 5 times - here we change it to -1, which makes it infinite
```
***



### Testing the bot
You're now ready to run the project. Go ahead, and send "!mybot hello" to your bot's account. It should reply!  
![](/_images/guides/connect-bot-1.png)

## Moving forward
While using `_client.AddMessageListener<ChatMessage>(OnChatMessage);` and `async void OnChatMessage(ChatMessage message)` works fine for testing, you probably want to develop fully fledged commands with your bot. Check [Commands System](xref:Guides.Commands.Intro) guides to see how!

Bot is a background program by nature, and as with any background program, knowing what is going on can be useful. For this reason Wolfringo has full logging support - check [Logging Guide](xref:Guides.Features.Logging) to see how to enable it.

You can also check [SimplePingBot Example](https://github.com/TehGM/Wolfringo/tree/master/Examples/SimplePingBot) (Normal Bot) or [HostedPingBot Example](https://github.com/TehGM/Wolfringo/tree/master/Examples/HostedPingBot) (.NET Generic Host/ASP.NET Core) for full example on basic Wolfringo usage. Feel free to also check [other example projects](https://github.com/TehGM/Wolfringo/tree/master/Examples)!