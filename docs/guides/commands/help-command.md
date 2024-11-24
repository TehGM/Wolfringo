---
uid: Guides.Commands.HelpCommand
title: Help Command
---

# Help Command
Since version 2.0.0, Wolfringo Commands System has new features that allow building a help command really easily. These features include special attributes for commands, as well as helper utility to create commands list, and even a default `help` command that can be enabled with a single line of code!

### Attributes
Wolfringo has a few built-in attributes that are automatically supported by both default help command and @TehGM.Wolfringo.Commands.Help.CommandsListBuilder. They should cover most common use cases.
- [\[DisplayName\]](xref:TehGM.Wolfringo.Commands.DisplayNameAttribute) - allows you to set how the command is displayed. If you don't add this attribute, the command will displayed as the pattern you set with [\[Command\] attribute](xref:TehGM.Wolfringo.Commands.CommandAttribute) or [\[RegexCommand\] attribute](xref:TehGM.Wolfringo.Commands.RegexCommandAttribute).
- [\[Summary\]](xref:TehGM.Wolfringo.Commands.SummaryAttribute) - a short description of what the command does. If you don't add this, the command will be listed without any description.
- [\[Hidden\]](xref:TehGM.Wolfringo.Commands.HiddenAttribute) - commands marked as hidden will not be listed by default help command or @TehGM.Wolfringo.Commands.Help.CommandsListBuilder. This attribute can also be put on the handler class - this will hide all commands in the handler.
- [\[HelpCategory\]](xref:TehGM.Wolfringo.Commands.HelpCategoryAttribute) - allows you to group the commands together. You can also provide the priority - categories with higher priority will appear before commands with lower priority. Can also be put on the handler class to make all commands in the handler share this category. Please note that commands without any category will be listed BEFORE the categories.
- [\[HelpOrder\]](xref:TehGM.Wolfringo.Commands.HelpOrderAttribute) - by default, commands will be ordered according to their [priorities](xref:Guides.Commands.Handlers#commands-priorities). If you want to change the order without affecting the priority, you can use [\[HelpOrder\] attribute](xref:TehGM.Wolfringo.Commands.HelpOrderAttribute).

You can see examples of how to use these attributes in [ExampleTransientCommandsHandler.cs](https://github.com/TehGM/Wolfringo/tree/master/Examples/SimpleCommandsBot/ExampleTransientCommandsHandler.cs) example.

## Default Help Command
Wolfringo Commands package comes with a default help command. This command:
- is triggered with prefix + "help" - for example `!help`;
- is transient;
- is hidden;
- has priority of [int.MinValue](xref:System.Int32.MinValue);
- will list commands even if they have no summary set;
- will group commands by category;
- will list commands without category first;
- will order commands based on [help order](xref:TehGM.Wolfringo.Commands.HelpOrderAttribute), or [priority](xref:Guides.Commands.Handlers#commands-priorities) if order not specified;
- will respond with "No commands found!" if it finds no commands to list.

This command is disabled by default, but don't worry - enabling it is just a single line change in Commands Options!

### [Without Wolfringo.Hosting (Normal Bot)](#tab/connecting-normal-bot)
```csharp
_client = new WolfClientBuilder()
    .WithCommands(commands =>
    {
        commands.WithPrefix("!");
        commands.WithPrefixRequirement(PrefixRequirement.Always);
        commands.WithCaseSensitivity(false);
        commands.WithDefaultHelpCommand();      // <---- call this method!
    })
    .Build();
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/connecting-hosted-bot)
```csharp
services.AddWolfringoCommands()
    .SetPrefix("!")           
    .SetPrefixRequirement(PrefixRequirement.Always)
    .SetCaseSensitive(false)
    .EnableDefaultHelpCommand();        // <---- call this method!
```
***

Once enabled, using this command will automatically create a list of your commands by using the attributes you set. Neat!

![](/_images/guides/help-command-1.png)

## Customizing the Commands List
If you want to add something to your help command, or change the command word, you can do it without much effort. Wolfringo includes @TehGM.Wolfringo.Commands.Help.CommandsListBuilder utility that will do all the heavy work for you, so all you need to do is create a new command.

Start off by disabling the default help command if you have it enabled. Then create a new command handler, with your help command inside - this can be done in the same way as [creating any other command](xref:Guides.Commands.Handlers).  
Then creating the commands itself is just a few simple steps:
1. Inject @TehGM.Wolfringo.Commands.ICommandsService and @TehGM.Wolfringo.Commands.CommandsOptions, either via constructor or to the command method itself;
2. Create @TehGM.Wolfringo.Commands.Help.CommandsListBuilder in your command method. Provide <xref:TehGM.Wolfringo.Commands.ICommandsService>.<xref:TehGM.Wolfringo.Commands.ICommandsService.Commands> via the constructor;
3. Set @TehGM.Wolfringo.Commands.Help.CommandsListBuilder properties to your liking;
4. Call @TehGM.Wolfringo.Commands.Help.CommandsListBuilder.GetCommandsList to get the string with your commands;
5. Send the response!

If you want, you can also add other text before or after the list - completely up to you!

Here is an example how it could look like:
```csharp
using TehGM.Wolfringo.Commands.Help;    // CommandsListBuilder is in this namespace

[CommandsHandler]
public class MyHelpCommandHandler
{
    private readonly ICommandsService _service;
    private readonly CommandsOptions _options;

    // injecting CommandsOptions and CommandsService via constructor
    public MyHelpCommandHandler(CommandsOptions options, ICommandsService service)
    {
        this._options = options;
        this._service = service;
    }

    [Command("help")]
    [Hidden]
    private async Task CmdHelpAsync(CommandContext context)
    {
        CommandsListBuilder builder = new CommandsListBuilder(this._service.Commands);   // create builder
        builder.PrependedPrefix = this._options.Prefix;     // set your prefix - here using the value from CommandsOptions
        builder.SpaceCategories = true;                     // set whether there should be additional spaces between categories
        builder.SummarySeparator = " == ";                  // string that separates command name and summary. Default is "    - " (4 spaces, dash, and one more space).
        builder.ListCommandsWithoutSummaries = true;        // if set to false, commands without [Summary] set will not be listed

        string result = builder.GetCommandsList();          // build the list
        if (string.IsNullOrWhiteSpace(result))              // if no commands found, the result will be empty
        {
            await context.ReplyTextAsync("Sorry, I found no commands to list! :( ");
            return;
        }

        // now you can edit your response if you want to!
        string response = result + "\n\nThank you for using my super bot!";

        // finally, send your response
        await context.ReplyTextAsync(response);
    }
}
```

This wasn't so scary, right? As you can see on the screenshot below, the help command is now customized - separator is different, and then there's a thank you line, too!

![](/_images/guides/help-command-2.png)

Of course you can do much more than this - this help command is now like any other command, so you can do anything that you can do in "normal" commands!

## Without CommandsListBuilder
If you need even more customizability, you don't need to use @TehGM.Wolfringo.Commands.Help.CommandsListBuilder at all. Instead you can use [CommandsService.Commands](xref:TehGM.Wolfringo.Commands.ICommandsService.Commands) enumerable to get all loaded @TehGM.Wolfringo.Commands.Initialization.ICommandInstanceDescriptor, and then use extension methods in `TehGM.Wolfringo.Commands` namespace to get values from the attributes by yourself. This allows you to effectively build your own "CommandsListBuilder".

An example (and very simple) help command could look like this:
```csharp
[Command("help")]
[Hidden]
private async Task CmdHelpAsync(CommandContext context)
{
    string prefix = this._options.Prefix;
    StringBuilder builder = new StringBuilder();
    foreach (ICommandInstanceDescriptor command in this._service.Commands)
    {
        bool isHidden = command.IsHidden();
        if (isHidden)
            continue;       // skip hidden commands

        string displayName = command.GetDisplayName();
        string summary = command.GetSummary();
        HelpCategoryAttribute category = command.GetHelpCategory(); // note - will return null if attribute is not present

        builder.AppendLine($"{prefix}{displayName} - {summary} - {category?.Name}");
    }

    await context.ReplyTextAsync(builder.ToString());
}
```

Of course the implementation above is very primitive, but it shows how you can easily get any help command you want.

You can even create your custom attributes - there's a special [GetAttribute\<T\>](xref:TehGM.Wolfringo.Commands.CommandInstanceDescriptorExtensions.GetAttribute``1(TehGM.Wolfringo.Commands.Initialization.ICommandInstanceDescriptor,System.Boolean)) extension method that will allow you to get any other attribute on your command. You can also set `includeHandlerAttributes` parameter to true - if you do this, the method will check attributes on the handler as well (this is how for example how [\[Hidden\] attribute](xref:TehGM.Wolfringo.Commands.HiddenAttribute) works when it's set on either method or the class)!  
If the attribute is not found, the method will return `null`.
```csharp
MyCustomAttribute customValue;
customValue = command.GetAttribute<MyCustomAttribute>();        // checks method only
customValue = command.GetAttribute<MyCustomAttribute>(true);    // checks method, and if not found, then handler class
if (customValue != null)    // if attribute was not found, null will be returned
    DoSomething();
```

All attributes retrieved when using @TehGM.Wolfringo.Commands.CommandInstanceDescriptorExtensions are lazy loaded, and are cached internally. This means that they'll be really fast after first use, but will not use memory if never used!

@TehGM.Wolfringo.Commands.Help.CommandsListBuilder of course uses a fairly more complex logic, so if you want to use it as reference, check [CommandsListBuilder.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Help/CommandsListBuilder.cs) on GitHub.