---
uid: Guides.Customizing.Client.Messages
title: Customizing Wolfringo - Custom Messages
---

# Custom Messages in Wolfringo
Wolfringo works with strongly typed messages - you can see all that are built-in [here](xref:TehGM.Wolfringo.Messages). They're serialized by the client using serializers.  
The list should be moderately up-to-date, but if you want to add a new message supported by WOLF without waiting for Wolfringo update, or want to customize the built-in messages behave, you have an option to do so!

## New Message Type
Adding a completely new message type is really easy in Wolfringo - all you need to do is create a new class that implements @TehGM.Wolfringo.IWolfMessage, set its event name (it'll be used both for serialization and by WOLF server), and any other data it requires. You can use Newtonsoft.JSON to modify serialization at will.  
For example, here's implementation of @TehGM.Wolfringo.Messages.PrivateChatHistoryMessage:
```csharp
[ResponseType(typeof(ChatHistoryResponse))]
public class PrivateChatHistoryMessage : IWolfMessage, IHeadersWolfMessage
{
    [JsonIgnore]
    public string EventName => MessageEventNames.MessagePrivateHistoryList;
    [JsonIgnore]
    public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
    {
        { "version", 2 }
    };

    [JsonProperty("id")]
    public uint UserID { get; private set; }
    [JsonProperty("timestampEnd", NullValueHandling = NullValueHandling.Ignore)]
    public WolfTimestamp? BeforeTime { get; private set; }

    [JsonConstructor]
    protected PrivateChatHistoryMessage() { }

    public PrivateChatHistoryMessage(uint userID, WolfTimestamp? before)
    {
        this.UserID = userID;
        this.BeforeTime = before;
    }
}
```

If message is being sent, you don't need to "nest" `body` property - by default, Wolfringo's built-in serializers will automatically handle that for you. If the message type needs `headers` property, simply implement @TehGM.Wolfringo.IHeadersWolfMessage - it'll cause `headers` property to also be handled by Wolfringo by default. For messages received from the server, Wolfringo will also handle `body.extended`, `body.base`, `metadata` and `body.metadata` properties for you. If however you need any of the receive-only properties, or completely different ones, you'll need to either "nest" them in your class, or register a [custom serializer](xref:Guides.Customizing.Client.Messages#custom-messageserializer) (see "Custom MessageSerializer" below).

## Registering MessageSerializer
@TehGM.Wolfringo.WolfClient needs to have means to translate @TehGM.Wolfringo.IWolfMessage.EventName to a concrete serializer. If you try sending the message without mapping a serializer, @TehGM.Wolfringo.WolfClient will log a warning, and attempt to use a [fallback one](xref:TehGM.Wolfringo.Messages.Serialization.DefaultMessageSerializer`1). But if the message is received from the server, or you simply want to suppress the warning, you'll need to register it. You can use [DefaultMessageSerializer\<T\>](xref:TehGM.Wolfringo.Messages.Serialization.DefaultMessageSerializer`1), unless you need custom serialization logic (which might be required in some cases due to the design of WOLF protocol) - in which case, you can register your custom one.

Registration of the @TehGM.Wolfringo.Messages.Serialization.IMessageSerializer is done by passing [ISerializerProvider<string, IMessageSerializer>](xref:TehGM.Wolfringo.Messages.Serialization.ISerializerProvider`2) to @TehGM.Wolfringo.WolfClient constructor. The default @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProvider uses a dictionary map, so for most use cases, you don't even need to create a custom one. 

### [Without Wolfringo.Hosting (Normal Bot)](#tab/connecting-normal-bot)
1. Manually create an instance of @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProviderOptions.
2. Add your serializer to @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProviderOptions.Serializers dictionary.
3. Create a new instance of @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProvider, passing your options instance via constructor
4. Pass your @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProvider instance to @TehGM.Wolfringo.WolfClient constructor.
```csharp
MessageSerializerProviderOptions messageSerializerProviderOptions = new MessageSerializerProviderOptions();
messageSerializerProviderOptions.Serializers["MyCustomMessageEventName"] = new DefaultMessageSerializer<MyCustomMessage>();
MessageSerializerProvider messageSerializerProvider = new MessageSerializerProvider(messageSerializerProviderOptions);
_client = new WolfClient(log, messageSerializers: messageSerializerProvider);
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/connecting-hosted-bot)
1. Configure @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProviderOptions.
2. Add your serializer to @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProviderOptions.Serializers dictionary.
```csharp
services.Configure<MessageSerializerProviderOptions>(options =>
{
    options.Serializers["MyCustomMessageEventName"] = new DefaultMessageSerializer<MyCustomMessage>();
});
```

***

>[!TIP]
> If default @TehGM.Wolfringo.Messages.Serialization.MessageSerializerProvider doesn't suffice your needs, you can create a [custom one](xref:Guides.Customizing.Client.Messages#custom-messageserializer) (see "Custom MessageSerializer" below) and provide it to @TehGM.Wolfringo.WolfClient as explained in [Introduction](xref:Guides.Customizing.Intro).
> See [MessageSerializerProvider.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Core/Messages/Serialization/MessageSerializerProvider.cs) and [MessageSerializerProviderOptions](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Core/Messages/Serialization/MessageSerializerProviderOptions.cs) on GitHub for reference.

## Custom MessageSerializer
If your message needs more complex serialization than [DefaultMessageSerializer\<T\>](xref:TehGM.Wolfringo.Messages.Serialization.DefaultMessageSerializer`1) provides, you can create a custom one by implementing @TehGM.Wolfringo.Messages.Serialization.IMessageSerializer (or even inheriting from [DefaultMessageSerializer\<T\>](xref:TehGM.Wolfringo.Messages.Serialization.DefaultMessageSerializer`1)).

An example implementation of built-in @TehGM.Wolfringo.Messages.Serialization.TipAddMessageSerializer:
```csharp
public class TipAddMessageSerializer : DefaultMessageSerializer<TipAddMessage>
{
    public override IWolfMessage Deserialize(string eventName, SerializedMessageData messageData)
    {
        // deserialize message
        TipAddMessage result = (TipAddMessage)base.Deserialize(eventName, messageData);
        messageData.Payload.PopulateObject(result, "context", SerializationHelper.DefaultSerializer);
        messageData.Payload.PopulateObject(result, "body.context", SerializationHelper.DefaultSerializer);

        return result;
    }

    public override SerializedMessageData Serialize(IWolfMessage message)
    {
        SerializedMessageData result = base.Serialize(message);
        JObject body = result.Payload["body"] as JObject;
        if (body == null)
            return result;
        // metadata props
        JObject context = new JObject();
        // move properties from body to context
        SerializationHelper.MovePropertyIfExists(ref body, ref context, "id");
        SerializationHelper.MovePropertyIfExists(ref body, ref context, "type");
        if (context.HasValues)
            body.Add(new JProperty("context", context));

        return result;
    }
}
```

Once class is created, you need to [register your serializer](xref:Guides.Customizing.Client.Messages#registering-messageserializer) - follow the guide above, but instead adding `DefaultMessageSerializer<MyCustomMessage>`, add instance of your custom class.

> [!TIP]
> @TehGM.Wolfringo.Messages.Serialization.Internal.SerializationHelper in *TehGM.Wolfringo.Messages.Serialization.Internal* namespace provides a few useful methods, such as `FlattenCommonProperties` (for handling "body" etc) or `MovePropertyIfExists`.

## Responses
WOLF server always sends a response to each sent message. Wolfringo has a default @TehGM.Wolfringo.Messages.Responses.WolfResponse class, but if you're adding a message type that needs to get some data from the server's response, see [Custom Responses guide](xref:Guides.Customizing.Client.Responses).