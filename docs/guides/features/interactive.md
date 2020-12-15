---
uid: Guides.Features.Interactive
title: Interactive
---

# Interactive Commands in Wolfringo
[Wolfringo.Utilities.Interactive](https://www.nuget.org/packages/Wolfringo.Utilities.Interactive) provides helper methods to easily await next message by user, in group, or custom conditions by providing own Func delegate.

Interactive Utilities function by listening to <xref:TehGM.Wolfringo.IWolfClient>'s events, and completing the Task when a received message matches criteria. This can be useful if for example user is using a command that has multiple steps.

Interactive Utilities by default can await a private message from user, a message from user in a specified group, or any message in a specified group. To do this, use [AwaitNextPrivateByUserAsync](xref:TehGM.Wolfringo.InteractiveExtensions.AwaitNextPrivateByUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.TimeSpan,System.Threading.CancellationToken)), [AwaitNextGroupByUserAsync](xref:TehGM.Wolfringo.InteractiveExtensions.AwaitNextGroupByUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.UInt32,System.TimeSpan,System.Threading.CancellationToken)) and [AwaitNextInGroupAsync](xref:TehGM.Wolfringo.InteractiveExtensions.AwaitNextInGroupAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.TimeSpan,System.Threading.CancellationToken)), respectively.  
You can also provide custom criteria - check [Custom Criteria](#custom-criteria) below for details.

```csharp
private async void OnChatMessage(ChatMessage message)
{
    if (!message.IsPrivateMessage) 
        return;

    await _client.ReplyTextAsync(message, "Ready? Set? Go!");
    DateTime startTime = DateTime.UtcNow;
    ChatMessage response = await _client.AwaitNextPrivateByUserAsync(message.SenderID.Value);

    double userSpeed = (DateTime.UtcNow - startTime).TotalSeconds;
    await _client.ReplyTextAsync(response, $"Congrats, you replied within {userSpeed}!");
}
```

> This feature was inspired by the same feature in [DSharpPlus](https://dsharpplus.github.io/articles/interactivity.html) library for Discord.  
> This is a neat idea that I directly borrowed, so figured a credit is due. :) 

### Timeouts
You most likely do not want to keep waiting for user's response forever - this could cause memory leaks, groups being blocked, etc. For this reason, you most likely want a timeout on your interactive commands.

Interactive extension methods take @System.TimeSpan as one of the parameters. If no received message matches criteria within that time, the interactive method will simply return `null`.

```csharp
private async void OnChatMessage(ChatMessage message)
{
    // ...
    ChatMessage response = await _client.AwaitNextPrivateByUserAsync(message.SenderID.Value);

    if (response == null) // if response message is null, it timed out
        await _client.ReplyTextAsync(message, "Aww, too slow. :(");
    else
    {
        // proceed as normal
    }
}
```

> [!TIP]
> Like most of async methods in Wolfringo, Interactive Utilities support @System.Threading.CancellationToken - combined with @System.Threading.CancellationTokenSource, you can implement custom timeouts if simple @System.TimeSpan does not suit your needs.
> Fun-fact: internally, that's exactly what Wolfringo does with the provided @System.TimeSpan.

### Custom Criteria
If the custom criteria do not match your needs, you can use [AwaitNextAsync&lt;T&gt;](xref:TehGM.Wolfringo.InteractiveExtensions.AwaitNextAsync``1(TehGM.Wolfringo.IWolfClient,System.Func{``0,System.Boolean},System.TimeSpan,System.Threading.CancellationToken)) which takes `Func<T, bool>` as one of parameters. That delegate should return true if message matches your custom criteria, otherwise it should return false.

The example below will await next image sent by the same user that sent the first message - regardless if the message is sent in private, or any of the groups.

```csharp
private async void OnChatMessage(ChatMessage message)
{
    // ...
    ChatMessage response = await _client.AwaitNextAsync<ChatMessage>((msg) =>
    {
        if (msg.SenderID.Value != message.SenderID.Value)
            return false;
        if (!msg.IsImage)
            return false;
        return true;
    });
    
    // proceed as normal
}
```

> [!TIP]
> `T` in [AwaitNextAsync&lt;T&gt;](xref:TehGM.Wolfringo.InteractiveExtensions.AwaitNextAsync``1(TehGM.Wolfringo.IWolfClient,System.Func{``0,System.Boolean},System.TimeSpan,System.Threading.CancellationToken)) can be any @TehGM.Wolfringo.Messages.IChatMessage. This enables usage of interactivity for other scenarios than just receiving a @TehGM.Wolfringo.Messages.ChatMessage.