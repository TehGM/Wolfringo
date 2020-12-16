---
uid: Guides.Features.Sender
title: Sender
---

# Sender Utility
This documentation mentions "Sender Utilitility" multiple times. This 'utility' is actually a set of extension classes for any @TehGM.Wolfringo.IWolfClient that handle common sending functionalities, and abstract concerns like [Message classes](xref:TehGM.Wolfringo.Messages) or [using cache](xref:Guides.Features.Caching#accessing-caches).

[Sender](xref:TehGM.Wolfringo.Sender) class itself is included in [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities) package (which is automatically included if you install [Wolfringo](https://www.nuget.org/packages/Wolfringo) metapackage), and is in *TehGM.Wolfringo* namespace so it's always easy to access when using any @TehGM.Wolfringo.IWolfClient.

> [!TIP]
> All of the methods in @TehGM.Wolfringo.Sender class support optional @System.Threading.CancellationToken.

## Logging in and out
Logging in is one of the most important steps your bot will need to take in order to work. This can be done using [LoginAsync](xref:TehGM.Wolfringo.Sender.LoginAsync(TehGM.Wolfringo.IWolfClient,System.String,System.String,TehGM.Wolfringo.WolfLoginType,System.Threading.CancellationToken)). This method returns a @TehGM.Wolfringo.Messages.Responses.LoginResponse, which includes profile of the user that logged in with @TehGM.Wolfringo.Messages.Responses.LoginResponse.User property.  
Once logged in, you most likely want to subscribe to incoming messages - otherwise, your client won't get any of them. You can do it using [SubscribeAllMessagesAsync](xref:TehGM.Wolfringo.Sender.SubscribeAllMessagesAsync(TehGM.Wolfringo.IWolfClient,System.Threading.CancellationToken)) method.  
After logging in, you can set your [online state](xref:TehGM.Wolfringo.WolfOnlineState) (such as Online/Busy/Invisible etc) after logging in using [SetOnlineStateAsync](xref:TehGM.Wolfringo.Sender.SetOnlineStateAsync(TehGM.Wolfringo.IWolfClient,TehGM.Wolfringo.WolfOnlineState,System.Threading.CancellationToken)) method.
```csharp
private static async void OnWelcome(WelcomeEvent message)
{
    // if reusing the token, user might be already logged in, so check that before requesting login
    if (message.LoggedInUser == null)
    {
        // note: it is recommended to not hardcode username and password, and use .gitignore-d config file instead
        await _client.LoginAsync("BotEmail", "BotPassword", WolfLoginType.Email);
    }
    await _client.SubscribeAllMessagesAsync();      // without this, bot will not receive any messages
    await _client.SetOnlineStateAsync(WolfOnlineState.Busy);    // this is optional - if you don't do this, you'll login as online
}
```

Logging out is also simple - just use [LogoutAsync](xref:TehGM.Wolfringo.Sender.LogoutAsync(TehGM.Wolfringo.IWolfClient,System.Threading.CancellationToken)) method!
```csharp
await _client.LogoutAsync();
```

## Sending messages
Sending messages is 2nd most important task almost any bot will perform, so naturally Sender Utility has methods for that as well.  
There are numerous methods for this in @TehGM.Wolfringo.Sender class:
- [SendPrivateTextMessageAsync](xref:TehGM.Wolfringo.Sender.SendPrivateTextMessageAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.String,System.Threading.CancellationToken)) - sends a private message to a user, with text content.
- [SendGroupTextMessageAsync](xref:TehGM.Wolfringo.Sender.SendGroupTextMessageAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.String,System.Threading.CancellationToken)) - sends a message to a group, with text content.
- [SendPrivateImageMessageAsync](xref:TehGM.Wolfringo.Sender.SendPrivateImageMessageAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - sends a private message to a user, with image content.
- [SendGroupImageMessageAsync](xref:TehGM.Wolfringo.Sender.SendGroupImageMessageAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - sends a message to a group, with image content.
- [SendPrivateVoiceMessageAsync](xref:TehGM.Wolfringo.Sender.SendPrivateVoiceMessageAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - sends a private message to a user, with voice content.
- [SendGroupVoiceMessageAsync](xref:TehGM.Wolfringo.Sender.SendGroupVoiceMessageAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - sends a message to a group, with voice content.

```csharp
byte[] myImageBytes = ...

await _client.SendPrivateTextMessageAsync(1234, "I will send an image in group ID 4321!");
await _client.SendGroupImageMessageAsync(4321, myImageBytes);
```

There also are methods that take a @TehGM.Wolfringo.Messages.ChatMessage as a parameter - these will send a message to pm or group, depending on the type of incoming message:
- [ReplyTextAsync](xref:TehGM.Wolfringo.Sender.ReplyTextAsync(TehGM.Wolfringo.IWolfClient,TehGM.Wolfringo.Messages.ChatMessage,System.String,System.Threading.CancellationToken)) - replies with a text message.
- [ReplyImageAsync](xref:TehGM.Wolfringo.Sender.ReplyImageAsync(TehGM.Wolfringo.IWolfClient,TehGM.Wolfringo.Messages.ChatMessage,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - replies with an image.
- [ReplyVoiceAsync](xref:TehGM.Wolfringo.Sender.ReplyVoiceAsync(TehGM.Wolfringo.IWolfClient,TehGM.Wolfringo.Messages.ChatMessage,System.Collections.Generic.IEnumerable{System.Byte},System.Threading.CancellationToken)) - replies with a voice message.

```csharp
private async void OnChatMessage(ChatMessage message)
{
    await _client.ReplyTextAsync(message, "Okay, will do!");
}
```

All of these tasks return @TehGM.Wolfringo.Messages.Responses.ChatResponse - you can use it to get the server-assigned @TehGM.Wolfringo.Messages.Responses.ChatResponse.Timestamp, or check if a private message was @TehGM.Wolfringo.Messages.Responses.ChatResponse.SpamFiltered.

## Retrieving entities
@TehGM.Wolfringo.Sender class has multiple methods to retrieve entites. Most of these methods will check [cache](xref:Guides.Features.Caching) first - if the entity is cached, it won't be requested from the server.

#### WolfUser
[GetUserAsync](xref:TehGM.Wolfringo.Sender.GetUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Threading.CancellationToken)) is the primary method to retrieve a @TehGM.Wolfringo.WolfUser. The user will be retrieved by their ID.  
If you want to retrieve multiple users at once, it is recommended to use [GetUsersAsync](xref:TehGM.Wolfringo.Sender.GetUsersAsync(TehGM.Wolfringo.IWolfClient,System.Collections.Generic.IEnumerable{System.UInt32},System.Threading.CancellationToken)) instead.
```csharp
WolfUser oneUser = await _client.GetUserAsync(2644384);
IEnumerable<WolfUser> multipleUsers = await _client.GetUsersAsync(new uint[] { 2644384, 39404842 });
```

You can also retrieve bot's profile using [GetCurrentUserAsync](xref:TehGM.Wolfringo.Sender.GetCurrentUserAsync(TehGM.Wolfringo.IWolfClient,System.Threading.CancellationToken)), or bot's contacts using [GetContactListAsync](xref:TehGM.Wolfringo.Sender.GetContactListAsync(TehGM.Wolfringo.IWolfClient,System.Threading.CancellationToken)).

#### WolfGroup
[GetGroupAsync](xref:TehGM.Wolfringo.Sender.GetGroupAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Threading.CancellationToken)) is the primary method to retrieve a @TehGM.Wolfringo.WolfGroup. The group can be retrieved by its ID or name.  
If you want to retrieve multiple groups at once, it is recommended to use [GetGroupsAsync](xref:TehGM.Wolfringo.Sender.GetGroupsAsync(TehGM.Wolfringo.IWolfClient,System.Collections.Generic.IEnumerable{System.UInt32},System.Threading.CancellationToken)) instead.
```csharp
WolfGroup oneGroup = await _client.GetGroupAsync("wolf");
IEnumerable<WolfGroup> multipleGroups = await _client.GetGroupsAsync(new uint[] { 2, 1234 });
```

You can also retrieve all groups that the bot is in using [GetCurrentUserGroupsAsync](xref:TehGM.Wolfringo.Sender.GetCurrentUserGroupsAsync(TehGM.Wolfringo.IWolfClient,System.Threading.CancellationToken)).

> [!WARNING]
> Sender Utility will make an attempt to automatically retrieve all group's members when retrieving a group. Unfortunately there is a bug in WOLF protocol, which sometimes causes server to not send group members even when requested.  
> If you need to access <xref:TehGM.Wolfringo.WolfGroup>'s @TehGM.Wolfringo.WolfGroup.Members but the dictionary is empty, you can try requesting the group again - Sender Utility will notice that cached Members list is empty, and will attempt to retrieve it again.

## Admin Actions
If the user has sufficent privileges in a group, it can use Sender Utility to perform admin actions:
- [AdminUserAsync](xref:TehGM.Wolfringo.Sender.AdminUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.UInt32,System.Threading.CancellationToken)) - gives an another user Admin privileges in a group.
- [ModUserAsync](xref:TehGM.Wolfringo.Sender.ModUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.UInt32,System.Threading.CancellationToken)) - gives an another user Mod privileges in a group.
- [ResetUserAsync](xref:TehGM.Wolfringo.Sender.ResetUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.UInt32,System.Threading.CancellationToken)) - resets an another user in a group.
- [SilenceUserAsync](xref:TehGM.Wolfringo.Sender.AdminUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.UInt32,System.Threading.CancellationToken)) - silences an another user in a group.
- [KickUserAsync](xref:TehGM.Wolfringo.Sender.AdminUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.UInt32,System.Threading.CancellationToken)) - kicks an another user from a group.
- [BanUserAsync](xref:TehGM.Wolfringo.Sender.AdminUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.UInt32,System.Threading.CancellationToken)) - bans an another user from a group.

```csharp
[Command("kick")]
[GroupOnly]
[RequireBotGroupMod]
private async Task KickAsync(CommandContext context, uint userID)
{
    await context.Client.KickUserAsync(userID, context.Message.RecipientID);
}
```

## Updating Profiles
Updating bot's nickname (display name) and status is really easy - just use [UpdateNicknameAsync](xref:TehGM.Wolfringo.Sender.UpdateNicknameAsync(TehGM.Wolfringo.IWolfClient,System.String,System.Threading.CancellationToken)) and [UpdateStatusAsync](xref:TehGM.Wolfringo.Sender.UpdateStatusAsync(TehGM.Wolfringo.IWolfClient,System.String,System.Threading.CancellationToken)).
```csharp
await _client.UpdateNicknameAsync("MyBot");
await _client.UpdateStatusAsync("Hi! Send \"!mybot help\" to check what I can do!");
```

If you want to update bot's (or any groups' that the bot has admin privileges in) profiles, you can use special @TehGM.Wolfringo.Sender's methods that take action as their parameter. These actions provide a special builder instance, that will be executed to construct an update message.  
Sender Utility uses this approach to construct an update message that includes even data that will not be changed - this is required, as nulls would make WOLF server simply erase that data.
```csharp
await _client.UpdateProfileAsync(profile =>
{
    profile.About = "Hello, I am a bot! Feel free to send me \"!mybot help\" to check what I can do!".
    profile.Language = WolfLanguage.English;
});
```

Group updates function in a similar manner - but you need to also provide ID of the group that you want to update:
```csharp
await _client.UpdateGroupAsync(1234, group =>
{
    profile.Description = "Group for testing MyBot!";
    profile.Language = WolfLanguage.English;
});
```

## Other functionalities
I listed just the most common usages of the Sender Utility. @TehGM.Wolfringo.Sender class contains many more methods that you can use - check them in [API Reference](xref:TehGM.Wolfringo.Sender)!

### Next Steps
Now you know how to send messages using Wolfringo. However the server might return an error sometimes - for example when your bot has no admin privileges when it tries to perform an admin action. Check [Errors Handling guide](xref:Guides.Features.ErrorHandling) to see how to deal with them.

An another important feature that can improve your work with Wolfringo is [Commands System](xref:Guides.Commands.Intro), so make sure you check it out!

[Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities) also includes support for interactive commands - check [Interactive Commands guide](xref:Guides.Features.Interactive) for a tutorial.