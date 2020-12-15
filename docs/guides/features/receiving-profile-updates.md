---
uid: Guides.Features.ReceivingProfileUpdates
---

# Receiving Profile Updates
Whenever user updates something in their profile or someone changes something in a group profile, WOLF server dispatches an update event. However this event is only dispatched to clients that subscribed to said event.  
Wolfringo does not automatically subscribe to updates. Majority of bots won't need this feature, and leaving it opt-in improves performance and reduces network traffic for bots that don't need that information.

### Subscribing to updates
To subscribe to profile updates, simply request said profile from Wolfringo servers.

If you have Wolfringo.Utilities (included with Wolfringo metapackage), you can simply use [GetUserAsync](xref:TehGM.Wolfringo.Sender.GetUserAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Threading.CancellationToken))/[GetUsersAsync](xref:TehGM.Wolfringo.Sender.GetUsersAsync(TehGM.Wolfringo.IWolfClient,System.Collections.Generic.IEnumerable{System.UInt32},System.Threading.CancellationToken)) to get user, or [GetGroupAsync](xref:TehGM.Wolfringo.Sender.GetGroupAsync(TehGM.Wolfringo.IWolfClient,System.UInt32,System.Threading.CancellationToken))/[GetGroupsAsync](xref:TehGM.Wolfringo.Sender.GetGroupsAsync(TehGM.Wolfringo.IWolfClient,System.Collections.Generic.IEnumerable{System.UInt32},System.Threading.CancellationToken)) to get group - Sender utility will automatically subscribe to the entities you request.  
You can also subscribe to all users that your bot has added as contacts using [GetContactListAsync](xref:TehGM.Wolfringo.Sender.GetContactListAsync(TehGM.Wolfringo.IWolfClient,System.Threading.CancellationToken)). Unfortunately, currently the same is not possible for bot's groups.

If you don't use Sender utility and send messages directly using their classes constructors, any message constructor that supports entity subscribing has a boolean `subscribe`. By setting it to true, you will subscribe to that profile.

### Receiving updates
@TehGM.Wolfringo.WolfClient will automatically intercept user/group update messages and update internal caches accordingly. If you still want to execute some custom logic once the bot receives an update, you can add message listener for @TehGM.Wolfringo.Messages.UserUpdateEvent, @TehGM.Wolfringo.Messages.GroupUpdateEvent and @TehGM.Wolfringo.Messages.GroupMemberUpdateEvent.

```csharp
_client.AddMessageListener<UserUpdateEvent>(OnUserUpdate);
_client.AddMessageListener<GroupUpdateEvent>(OnGroupUpdate);
_client.AddMessageListener<GroupMemberUpdateEvent>(OnGroupMemberUpdate);

private static void OnUserUpdate(UserUpdateEvent message)
{
    // executed whenever an user you subscribed to updates profile
}

private static void OnGroupUpdate(GroupUpdateEvent message)
{
    // executed whenever a group you subscribed to updates profile
}

private static void OnGroupMemberUpdate(GroupMemberUpdateEvent message)
{
    // executed whenever a group member (of a group you subscribed to) is updated
    // note: this means user being admined, kicked etc - not the user's profile itself
}
```

## Profile Updates and Reconnection

Unfortunately, I am not currently able to tell if server requires re-subscribing when you reconnect. If you want to make sure that you're always subscribed, you can re-request profiles whenever you log in.

```csharp
_client.MessageSent += async (sender, e) =>
{
    if (e.Message.EventName.Equals(MessageEventNames.SecurityLogin))
    {
        // any of your subscribing logic here, for example:
        await _client.GetUsersAsync(_mySubscribedUsers);
        await _client.GetGroupsAsync(_mySubscribedGroups);
    }
};
```

It is recommended to limit requested entities, and to use [GetUsersAsync](xref:TehGM.Wolfringo.Sender.GetUsersAsync(TehGM.Wolfringo.IWolfClient,System.Collections.Generic.IEnumerable{System.UInt32},System.Threading.CancellationToken)) and [GetGroupsAsync](xref:TehGM.Wolfringo.Sender.GetGroupsAsync(TehGM.Wolfringo.IWolfClient,System.Collections.Generic.IEnumerable{System.UInt32},System.Threading.CancellationToken)) that take any IEnumerable>uint< instead of individual ID. If you spam the server too much, it may temporarily suspend your bot's account.
