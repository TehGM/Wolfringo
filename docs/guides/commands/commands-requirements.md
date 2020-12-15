---
uid: Guides.Commands.Requirements
---

# Commands Requirements
Very often a bot's command is designed to work only in groups, require that user is a mod or admin, or even that the bot itself is a mod or admin. While it is possible to implement the checks in command's method, these tasks are quite common, so a fair bit of boilerplate code would be required - especially if you create multiple bots.

Wolfringo Commands System aims to reduce the amount of boilerplate code, and that includes code required for such checks. For that reason, [Wolfringo.Commands](https://www.nuget.org/packages/Wolfringo.Commands) includes some of the most common checks in form of easy to use attributes. Wolfringo Commands System also allows easy implementation of own attributes for checks - see [Custom Requirements](xref:Guides.Commands.Requirements#custom-requirements) to see how to create one.

## How Command Requirements work
After selecting a @TehGM.Wolfringo.Commands.Attributes.CommandAttributeBase, such as [\[Command\]](xref:TehGM.Wolfringo.Commands.CommandAttribute) or [\[RegexCommand\]](xref:TehGM.Wolfringo.Commands.RegexCommandAttribute), it'll run [CheckAsync](xref:TehGM.Wolfringo.Commands.ICommandRequirement.CheckAsync(TehGM.Wolfringo.Commands.ICommandContext,System.IServiceProvider,System.Threading.CancellationToken)) method on each of the requirements. If any of the requirements return false, execution of a command will be aborted.

Because requirements are checked after a command is selected, and Wolfringo Commands System never executes more than one command per message, if any of the requirements fail, no other command will be executed for that message.

#### Error Messages
Requirements support error messages - if any is specified, in case of command execution being aborted, the message will be sent back to the user that invoked the command.  
Most of built-in requirements (with exception of [\[IgnoreBots\]](xref:TehGM.Wolfringo.Commands.IgnoreBotsAttribute)) have a default message specified. You can change that message freely by changing value of [ErrorMessage](xref:TehGM.Wolfringo.Commands.ICommandRequirement.ErrorMessage) property.  
You can also set ErrorMessage to `null` or empty string - in such case, no error response will be sent back by the bot.
```csharp
[Command("example")]
[GroupOnly(ErrorMessage = "(n) Please try in a group instead!")]
private async Task ExampleAsync() { }
```

#### Requirements on Handlers
Command Requirements can be specified on the handler as well. When a handler is marked with a requirement, ALL of the commands in that handler will have that requirement. In the following example, ExampleAsync will only be run for messages that called this command from a group.
```csharp
[CommandsHandler]
[GroupOnly]
private class ExampleHandler
{
    [Command("example")]
    private async Task ExampleAsync()
    {
        // command code here will only be ran for commands called from group chats
    }
}
```

#### Multiple Requirements
Multiple requirements can be specified on the same method or handler. When there are requirements on both, method and handler, they're automatically combined.  
When there is more than one requirement for a command, they all need to pass. If at least one is not passed, the command execution is aborted.
> [!WARNING]
> Be cautious when mixing requirements. Some requirements, like [\[GroupOnly\]](xref:TehGM.Wolfringo.Commands.GroupOnlyAttribute) and [\[PrivateOnly\]](xref:TehGM.Wolfringo.Commands.PrivateOnlyAttribute) for example, are mutually exclusive - if you set them both for the same command, that command will always be aborted!

## Built-in Requirements
Wolfringo Commands System has some requirements built-in for convenience.

| Requirement | Behaviour | Default Error Message | Notes |
| :---------: | --------- | --------------------- | ----- |
| [\[IgnoreBots\]](xref:TehGM.Wolfringo.Commands.IgnoreBotsAttribute) | Prevents official bots (with BOT tag) from executing the command. | `null` | *User-made bots don't have the tag, so can still use the command.* |
| [\[GroupOnly\]](xref:TehGM.Wolfringo.Commands.GroupOnlyAttribute) | Makes the command only usable in group chats. | "(n) This command can be used in groups only." | *Mutually exclusive with [\[PrivateOnly\]](xref:TehGM.Wolfringo.Commands.PrivateOnlyAttribute).* |
| [\[PrivateOnly\]](xref:TehGM.Wolfringo.Commands.PrivateOnlyAttribute) | Makes the command only usable in private messages. | "(n) This command can be used in PM only." | *Mutually exclusive with [\[GroupOnly\]](xref:TehGM.Wolfringo.Commands.GroupOnlyAttribute).* |
| [\[RequireGroupMod\]](xref:TehGM.Wolfringo.Commands.RequireGroupModAttribute) | Requires the user invoking the command to have Mod, Admin or Owner role. | "(n) You need to be at least a mod to execute this command." | *By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.* |
| [\[RequireGroupAdmin\]](xref:TehGM.Wolfringo.Commands.RequireGroupAdminAttribute) | Requires the user invoking the command to have Admin or Owner role. | "(n) You need to be at least an admin to execute this command." | *By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.* |
| [\[RequireGroupOwner\]](xref:TehGM.Wolfringo.Commands.RequireGroupOwnerAttribute) | Requires the user invoking the command to have Owner role. | "(n) You need to be an owner to execute this command." | *By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.* |
| [\[RequireBotGroupMod\]](xref:TehGM.Wolfringo.Commands.RequireBotGroupModAttribute) | Requires the bot to have Mod, Admin or Owner role. | "(n) I need to be at least a mod to execute this command." | *By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireBotGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.* |
| [\[RequireBotGroupAdmin\]](xref:TehGM.Wolfringo.Commands.RequireBotGroupAdminAttribute) | Requires the bot to have Admin or Owner role. | "(n) I need to be at least an admin to execute this command." | *By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireBotGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.* |
| [\[RequireBotGroupOwner\]](xref:TehGM.Wolfringo.Commands.RequireBotGroupOwnerAttribute) | Requires the bot to have Owner role. | "(n) I need to be an owner to execute this command." | *By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireBotGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.* |
| [\[RequireUserEntertainer\]](xref:TehGM.Wolfringo.Commands.RequireUserEntertainerAttribute) | Allows only Entertainer users (with ENTERTAINER tag) to run the command. | "(n) You need to be an Entertainer to execute this command." | *Some users might be entertainers even if they have a different tag - such as STAFF tag.* |
| [\[RequireUserStaff\]](xref:TehGM.Wolfringo.Commands.RequireUserStaffAttribute) | Allows only Staff users (with STAFF tag) to run the command. | "(n) You need to be a Staff to execute this command." | - |
| [\[RequireUserVolunteer\]](xref:TehGM.Wolfringo.Commands.RequireUserStaffAttribute) | Allows only Volunteer users (with VOLUNTEER tag) to run the command. | "(n) You need to be a Volunteer to execute this command." | *Some users might be volunteers even if they have a different tag - such as STAFF tag.* |
| [\[RequireMaximumReputation(double)\]](xref:TehGM.Wolfringo.Commands.RequireMaximumReputationAttribute) | Ensures that user calling the command has reputation level equal to or smaller than specified. | "(n) Your reputation is too high to execute this command." | *Care needed if used with [\[RequireMinimumReputation(double)\]](xref:TehGM.Wolfringo.Commands.RequireMinimumReputationAttribute).* |
| [\[RequireMinimumReputation(double)\]](xref:TehGM.Wolfringo.Commands.RequireMinimumReputationAttribute) | Ensures that user calling the command has reputation level equal to or greater than specified. | "(n) Your reputation is too low to execute this command." | *Care needed if used with [\[RequireMaximumReputation(double)\]](xref:TehGM.Wolfringo.Commands.RequireMaximumReputationAttribute).* |

## Custom Requirements
You can also create your own requirements easily. All you need to do is create a new class that inherits from @TehGM.Wolfringo.Commands.Attributes.CommandRequirementAttribute (from *TehGM.Wolfringo.Commands.Attributes* namespace), and attach the new attribute to your command.

Command Requirement's [CheckAsync](xref:TehGM.Wolfringo.Commands.ICommandRequirement.CheckAsync(TehGM.Wolfringo.Commands.ICommandContext,System.IServiceProvider,System.Threading.CancellationToken)) method has @System.IServiceProvider as one of its parameters. Whenever @TehGM.Wolfringo.Commands.CommandsService runs checks, its services will be provided via this parameter. You can use it to gain access to your own services - for example your Database class, or whatever else you might need! To check how to add your services to the provider, see [Dependency Injection guide](xref:Guides.Commands.DependencyInjection)!

The example below is taken directly from my Size Bot. It uses dependency injection to access one of database services (*IUserDataStore*), and then checks if user has administrative privileges within the bot - which allows to prevent unauthorized users from changing bot's settings, restarting it, etc:
```csharp
using TehGM.Wolfringo.Commands.Attributes;

public class RequireBotAdminAttribute : CommandRequirementAttribute
{
    public RequireBotAdminAttribute() : base()
    {
        ErrorMessage = "(n) You are not permitted to do this!";
    }

    public override async Task<bool> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
    {
        IUserDataStore userDataStore = services.GetRequiredService<IUserDataStore>();
        // check if user is bot admin
        UserData userData = await userDataStore.GetUserDataAsync(context.Message.SenderID.Value, cancellationToken).ConfigureAwait(false);
        return userData.IsBotAdmin;
    }
}
```

> [!TIP]
> You can also view this example on GitHub: https://github.com/TehGM/WolfBot-Size/blob/master/PicSizeCheckBot/CommandsExtensions/RequireBotAdminAttribute.cs

#### Specialized base classes
@TehGM.Wolfringo.Commands.Attributes.CommandRequirementAttribute is the default base class for commands requirements, and will be used for custom requirements most of the time.  
But *TehGM.Wolfringo.Commands.Attributes* namespace has a few other base attributes as well. These attributes can be used directly, or extended (by using inheritance) to make creating your own requirements easier.

##### RequireGroupPrivilegeAttribute
This attribute ensures that the user invoking the command has at least one of the specified group privileges. Its constructor takes @TehGM.Wolfringo.WolfGroupCapabilities. You can provide multiple flags, for example `WolfGroupCapabilities.Admin | WolfGroupCapabilities.Owner` - this will allow either Admin OR Owner to run the command.

By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.

Default error message is "(n) You don't have enough group privileges to execute this command.".

This attribute is a base class for [\[RequireGroupMod\]](xref:TehGM.Wolfringo.Commands.RequireGroupModAttribute), [\[RequireGroupAdmin\]](xref:TehGM.Wolfringo.Commands.RequireGroupAdminAttribute) and [\[RequireGroupOwner\]](xref:TehGM.Wolfringo.Commands.RequireGroupOwnerAttribute).


##### RequireBotGroupPrivilegeAttribute
This attribute ensures that the bot has at least one of the specified group privileges. Its constructor takes @TehGM.Wolfringo.WolfGroupCapabilities. You can provide multiple flags, for example `WolfGroupCapabilities.Admin | WolfGroupCapabilities.Owner` - this will allow command to be ran if the bot is either Admin OR Owner.

By default, this also makes the command group-only. You can set [IgnoreInPrivate](xref:TehGM.Wolfringo.Commands.Attributes.RequireBotGroupPrivilegeAttribute.IgnoreInPrivate) to true to make it work in PM for everyone.

Default error message is "(n) I don't have enough group privileges to execute this command.".

This attribute is a base class for [\[RequireBotGroupMod\]](xref:TehGM.Wolfringo.Commands.RequireBotGroupModAttribute), [\[RequireBotGroupAdmin\]](xref:TehGM.Wolfringo.Commands.RequireBotGroupAdminAttribute) and [\[RequireBotGroupOwner\]](xref:TehGM.Wolfringo.Commands.RequireBotGroupOwnerAttribute).

##### RequireUserPrivilegeAttribute
This attribute ensures that the user invoking the command has at least one of the specified account privileges. Its constructor takes @TehGM.Wolfringo.WolfPrivilege. You can provide multiple flags, for example `WolfPrivilege.Volunteer | WolfPrivilege.Staff` - this will allow either Volunteer OR Staff to run the command.

Default error message is "(n) You don't have enough user privileges to execute this command.".

This attribute is a base class for [\[RequireUserEntertainer\]](xref:TehGM.Wolfringo.Commands.RequireUserEntertainerAttribute), [\[RequireUserVolunteer\]](xref:TehGM.Wolfringo.Commands.RequireUserStaffAttribute) and [\[RequireUserStaff\]](xref:TehGM.Wolfringo.Commands.RequireUserStaffAttribute).