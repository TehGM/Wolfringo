---
uid: Guides.Features.ErrorHandling
---

# Handling Errors
Errors handling is very important to keep your bot running, and to diagnose any bugs and problems your bot might have. In Wolfringo, there are 2 types of errors: those that happen when you call some Wolfringo method, and those that occur on the background thread.

### Errors from your calls
Wolfringo will throw an exception whenever there is an error that is caused by your action. You can use try-catch block to handle them.

There is one special exception type defined by Wolfringo library - @TehGM.Wolfringo.MessageSendingException. This exception is thrown when WOLF server responds with a non-success status code to a message you sent. You can handle it like any other exception.

```csharp
try
{
    await _client.SendGroupTextMessageAsync(12345, "Hello!");
}
catch (MessageSendingException ex)
{
    IWolfMessage sentMessage = ex.SentMessage;
    IWolfResponse serverResponse = ex.Response;
    string errorMessage = ex.Message;
}
```

#### Errors in commands
If you enabled logging, any exceptions that occured in your [commands](xref:Guides.Commands.Handlers) will be automatically [logged](xref:Guides.Features.Logging) if you don't handle them yourself. Most exceptions are logged as error, with a few exceptions:

###### <xref:System.OperationCanceledException> (always)
Operation cancellations are used to control the flow of asynchronous applications. They're a normal and relatively common occurence, so they're logged as warning instead.

###### <xref:TehGM.Wolfringo.MessageSendingException> (conditionally)
Nearly all commands of your bot will send a response, but your bot might be silenced in the group. You can check that by requesting the group's profile and checking its members list to see what @TehGM.Wolfringo.WolfGroupCapabilities your bot has. But in many cases, this might be unnecessary and just make your commands code much longer and less maintainable. For this reason, Commands System will log this exception as warning instead of error if all of the following conditions are met:
- Server responded with @System.Net.HttpStatusCode.Forbidden (403).
- The response has @TehGM.Wolfringo.Messages.Responses.WolfResponse.ErrorCode with value @TehGM.Wolfringo.Messages.Responses.WolfErrorCode.LoginIncorrectOrCannotSendMessage (1).
- The message sent by the bot implements <xref:TehGM.Wolfringo.Messages.IChatMessage>.
- The message sent by the bot was sent to the same user (in case of PMs) or the same group (in case of groups) as where the command came from.

### Errors on background thread
Errors can sometimes occur from background thread. The best way to be aware of them is to enable [logging](xref:Guides.Features.Logging) - @TehGM.Wolfringo.WolfClient will log all errors it caught into the logger.

Additionally, you can subscribe to [IWolfClient.ErrorRaised](xref:TehGM.Wolfringo.IWolfClient.ErrorRaised) event. Its @System.UnhandledExceptionEventArgs contains @System.UnhandledExceptionEventArgs.ExceptionObject property, which is the exception that occured.