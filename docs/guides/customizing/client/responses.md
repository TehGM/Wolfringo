---
uid: Guides.Customizing.Client.Responses
title: Customizing Wolfringo - Custom Responses
---

# Custom Responses in Wolfringo
WOLF server always sends a response to each sent message. Much like with messages, Wolfringo works with strongly typed responses - you can see all that are built-in [here](xref:TehGM.Wolfringo.Messages.Responses). They're serialized by the client using serializers.  
The list should be moderately up-to-date, but if you want to add a new message supported by WOLF without waiting for Wolfringo update, or want to customize the built-in messages behave, you have an option to do so!

> [!TIP]
> Wolfringo has base @TehGM.Wolfringo.Messages.Responses.WolfResponse response class - if it offers all you need, you don't need to create a custom response type at all!

## New Response Type
Adding a completely new response type is really easy in Wolfringo - all you need to do is create a new class that implements @TehGM.Wolfringo.Messages.Responses.IWolfResponse (or inherit from @TehGM.Wolfringo.Messages.Responses.WolfResponse default class) and add any other properties it requires. You can use Newtonsoft.JSON to modify serialization at will.  
For example, here's implementation of @TehGM.Wolfringo.Messages.Responses.ChatResponse:
```csharp
public class ChatResponse : WolfResponse, IWolfResponse
{
    [JsonProperty("uuid")]
    public Guid ID { get; private set; }
    [JsonProperty("timestamp")]
    public WolfTimestamp Timestamp { get; private set; }
    [JsonProperty("isSpam")]
    public bool SpamFiltered { get; private set; }

    [JsonConstructor]
    protected ChatResponse() : base() { }
}
```

You don't need to "nest" `body`, `headers`, `body.extended`, `body.base`, `metadata` and `body.metadata` properties - by default, Wolfringo's built-in serializers will automatically handle that for you. If however you need to "nest" any different properties, you'll need to either "nest" them in your class, or register a [custom serializer](xref:Guides.Customizing.Client.Responses#custom-responseserializer) (see "Custom ResponseSerializer" below).

## Mapping Message to Response Type
### Custom Message Types
Mapping response type of a custom message type (one that you can edit code of) is really easy - simply use [\[ResponseType\] attribute](xref:TehGM.Wolfringo.Messages.Responses.ResponseTypeAttribute) on the class.
```csharp
[ResponseType(typeof(MyCustomResponse))]
public class MyCustomMessage : IWolfMessage
{
    // ... other code here
}
```

### Built-in Message Types
Wolfringo allows you to overwrite the response type of built-in messages if you wish to do so. To do that, you'll need to create a new @TehGM.Wolfringo.Messages.Responses.IResponseTypeResolver.  
The default @TehGM.Wolfringo.Messages.Responses.ResponseTypeResolver in Wolfringo checks [\[ResponseType\] attribute](xref:TehGM.Wolfringo.Messages.Responses.ResponseTypeAttribute) on the message class, and if there's none, it'll use fallback - which is whatever is provided to [WolfClient.SendAsync\<TResponse\> method](xref:TehGM.Wolfringo.WolfClient.SendAsync``1(TehGM.Wolfringo.IWolfMessage,System.Threading.CancellationToken)). It is recommended that you still respect [\[ResponseType\] attribute](xref:TehGM.Wolfringo.Messages.Responses.ResponseTypeAttribute) after handling the "overwrite" for your custom one, to prevent breaking. You can see implementation of [ResponseTypeResolver on GitHub](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Core/Messages/Responses/ResponseTypeResolver.cs) for reference and guide.

Once you have programmed your custom @TehGM.Wolfringo.Messages.Responses.IResponseTypeResolver implementation, you need to register it as a service with @TehGM.Wolfringo.WolfClient - see [Introduction](xref:Guides.Customizing.Intro) to see how to do that.

## Registering ResponseSerializer
@TehGM.Wolfringo.WolfClient needs to have means to translate Response type to a concrete serializer. If you try sending the message without mapping a response serializer, @TehGM.Wolfringo.WolfClient will log a warning, and attempt to use a [fallback one](xref:TehGM.Wolfringo.Messages.Serialization.DefaultResponseSerializer). But if the message is received from the server, or you simply want to suppress the warning, you'll need to register it. You can use [DefaultResponseSerializer](xref:TehGM.Wolfringo.Messages.Serialization.DefaultResponseSerializer), unless you need custom deserialization logic (which might be required in some cases due to the design of WOLF protocol) - in which case, you can register your custom one.

Registration of the @TehGM.Wolfringo.Messages.Serialization.IResponseSerializer is done by passing [ISerializerProvider<Type, IResponseSerializer>](xref:TehGM.Wolfringo.Messages.Serialization.ISerializerProvider`2) to @TehGM.Wolfringo.WolfClient constructor. The default @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProvider uses a dictionary map, so for most use cases, you don't even need to create a custom one. 

### [Without Wolfringo.Hosting (Normal Bot)](#tab/connecting-normal-bot)
1. Manually create an instance of @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProviderOptions.
2. Add your serializer to @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProviderOptions.Serializers dictionary.
3. Create a new instance of @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProvider, passing your options instance via constructor
4. Pass your @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProvider instance to @TehGM.Wolfringo.WolfClient constructor.
```csharp
ResponseSerializerProviderOptions responseSerializerProviderOptions = new ResponseSerializerProviderOptions();
responseSerializerProviderOptions.Serializers[typeof(MyCustomResponse)] = new DefaultResponseSerializer();
ResponseSerializerProvider responseSerializerProvider = new ResponseSerializerProvider(responseSerializerProviderOptions);
_client = new WolfClient(log, responseSerializers: responseSerializerProvider);
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/connecting-hosted-bot)
1. Configure @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProviderOptions.
2. Add your serializer to @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProviderOptions.Serializers dictionary.
```csharp
services.Configure<ResponseSerializerProviderOptions>(options =>
{
    options.Serializers[typeof(MyCustomResponse)] = new DefaultResponseSerializer();
});
```

***

>[!TIP]
> If default @TehGM.Wolfringo.Messages.Serialization.ResponseSerializerProvider doesn't suffice your needs, you can create a [custom one](xref:Guides.Customizing.Client.Responses#custom-responseserializer) (see "Custom ResponseSerializer" below) and provide it to @TehGM.Wolfringo.WolfClient as explained in [Introduction](xref:Guides.Customizing.Intro).
> See [ResponseSerializerProvider.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Core/Messages/Serialization/ResponseSerializerProvider.cs) and [ResponseSerializerProviderOptions](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Core/Messages/Serialization/ResponseSerializerProviderOptions.cs) on GitHub for reference.

## Custom ResponseSerializer
If your message needs more complex serialization than [DefaultResponseSerializer](xref:TehGM.Wolfringo.Messages.Serialization.DefaultResponseSerializer) provides, you can create a custom one by implementing @TehGM.Wolfringo.Messages.Serialization.IResponseSerializer (or even inheriting from [DefaultResponseSerializer](xref:TehGM.Wolfringo.Messages.Serialization.DefaultResponseSerializer)).

An example implementation of built-in @TehGM.Wolfringo.Messages.Serialization.TipAddMessageSerializer:
```csharp
public class TipDetailsResponseSerializer : DefaultResponseSerializer, IResponseSerializer
{
    private static readonly Type _tipDetailsResponseType = typeof(TipDetailsResponse);

    public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
    {
        JToken payload = GetResponseJson(responseData.Payload).DeepClone();
        JArray charmList = payload.SelectToken("body.list") as JArray;
        foreach (JObject charmDetails in charmList.Children().Cast<JObject>())
        {
            JToken value = charmDetails["charmId"];
            if (value != null)
            {
                charmDetails.Remove("charmId");
                charmDetails.Add("id", value);
            }
        }
        return (TipDetailsResponse)base.Deserialize(responseType, new SerializedMessageData(payload, responseData.BinaryMessages));
    }

    protected override void ThrowIfInvalidType(Type responseType)
    {
        base.ThrowIfInvalidType(responseType);
        if (!_tipDetailsResponseType.IsAssignableFrom(responseType))
            throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_tipDetailsResponseType.FullName}", nameof(responseType));
    }
}
```

Once class is created, you need to [register your serializer](xref:Guides.Customizing.Client.Responses#registering-responseserializer) - follow the guide above, but instead adding `DefaultResponseSerializer`, add instance of your custom class.

> [!TIP]
> @TehGM.Wolfringo.Messages.Serialization.Internal.SerializationHelper in *TehGM.Wolfringo.Messages.Serialization.Internal* namespace provides a few useful methods, such as `FlattenCommonProperties` (for handling "body" etc) or `MovePropertyIfExists`.