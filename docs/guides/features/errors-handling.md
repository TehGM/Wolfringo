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

> [!TIP]
> Exceptions that occured in your [commands](xref:Guides.Commands.Handlers) will be automatically [logged](xref:Guides.Features.Logging) if you don't handle them yourself (and of course, enable logging).
> Exceptions from event handlers need to be handled by you, however.

### Errors on background thread
Errors can sometimes occur from background thread. The best way to be aware of them is to enable [logging](xref:Guides.Features.Logging) - @TehGM.Wolfringo.WolfClient will log all errors it caught into the logger.

Additionally, you can subscribe to [IWolfClient.ErrorRaised](xref:TehGM.Wolfringo.IWolfClient.ErrorRaised) event. Its @System.UnhandledExceptionEventArgs contains @System.UnhandledExceptionEventArgs.ExceptionObject property, which is the exception that occured.