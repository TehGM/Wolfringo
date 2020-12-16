
# Wolfringo
[![Nuget](https://img.shields.io/nuget/v/Wolfringo)](https://www.nuget.org/packages/Wolfringo/) [![GitHub top language](https://img.shields.io/github/languages/top/TehGM/Wolfringo)](https://github.com/TehGM/Wolfringo) [![GitHub](https://img.shields.io/github/license/TehGM/Wolfringo)](LICENSE) [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/TehGM/Wolfringo/.NET%20Core%20Build)](https://github.com/TehGM/Wolfringo/actions) [![GitHub issues](https://img.shields.io/github/issues/TehGM/Wolfringo)](https://github.com/TehGM/Wolfringo/issues)

This is a .NET library for WOLF (previously Palringo).

This library is designed with extensibility through Dependency Injection in mind, and is compatible with ASP.NET Core and other .NET Core Hosting scenarios through [Wolfringo.Hosting](https://www.nuget.org/packages/Wolfringo.Hosting/) package.

Library works with strongly-typed messages and responses, that are serialized when sending and deserialized when receiving. Message listeners can be invoked by message type, giving full benefit of strong typing. Additionally, [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities/) package provides a Sender extensions class, which abstracts common sending tasks. Utilities package is included by default with [Wolfringo](https://www.nuget.org/packages/Wolfringo/) meta-package.

Wolfringo provides a built in Commands System. Commands System uses attributes to mark commands, which greatly reduces amount of boilerplace code needed.  
The Commands System follows the design principles of entire Wolfringo library, and therefore is easily extensible and easily customizable thanks to Dependency Injection.

### Documentation
A full [Documentation](https://tehgm.github.io/Wolfringo) is now available - it includes Tutorials and Guides, as well as full API Reference.

### Download
Most recent versions of this package are downloadable via [nuget.org](https://www.nuget.org/packages/Wolfringo/)!

1. Install package in your project
    ```cli
    Install-Package Wolfringo
    ```
2. *(.NET Core Host/ASP.NET Core only)* Install Wolfringo.Hosting package
    ```cli
    Install-Package Wolfringo.Hosting
    ```

#### Older versions
Older versions are available through GitHub Packages. See [Installation guide in Documentation](https://tehgm.github.io/Wolfringo/guides/getting-started/installation#older-versions) for steps to install version before v0.4.0.


## Extending the client
#### Serializer providers
Client uses power of Dependency Injection to allow customizability. The client accepts optional Message and Response Serializer providers which are used for serializing and deserializing the message and response objects. You can inject own instance of the map to change mapping, or even add new types through their options if it's required.

You can see [MessageSerializerProvider](Wolfringo.Core/Messages/Serialization/MessageSerializerProvider.cs), [ResponseSerializerProvider](Wolfringo.Core/Messages/Serialization/DefaultResponseSerializerProvider.cs), [DefaultMessageSerializer](Wolfringo.Core/Messages/Serialization/Serializers/DefaultMessageSerializer.cs) and [DefaultResponseSerializer](Wolfringo.Core/Messages/Serialization/Serializers/DefaultResponseSerializer.cs) for examples of default base implementations.

To check the default mappings, see [MessageSerializerProviderOptions](Wolfringo.Core/Messages/Serialization/MessageSerializerProviderOptions.cs) and [ResponseSerializerProviderOptions](Wolfringo.Core/Messages/Serialization/DefaultResponseSerializerProviderOptions.cs).

#### Overriding client methods
Client automatically caches the entities based on message/response type. If you add a new type that needs to support this, you must create a new client class inheriting from [WolfClient](Wolfringo.Core/WolfClient.cs). You can override `OnMessageSentInternalAsync` method to change behaviour for sent messages and received responses, and `OnMessageReceivedInternalAsync` method to change behaviour for received events and messages.

> Note: it's recommended to still call base method after own implementation to prevent accidentally breaking default behaviour. Overriding these methods should be handled with caution.

#### Determining response type for sent message
[WolfClient](Wolfringo.Core/WolfClient.cs) needs to know how to deserialize message's response, and to determine the type, it uses an [IResponseTypeResolver](Wolfringo.Core/Messages/Responses/IResponseTypeResolver.cs) to select the type that will be used with response serializer mappings. This interface can be passed into the client constructor. If null or none is passed in, [DefaultResponseTypeResolver](Wolfringo.Core/Messages/Responses/DefaultResponseTypeResolver.cs) will be used automatically.

[DefaultResponseTypeResolver](Wolfringo.Core/Messages/Responses/DefaultResponseTypeResolver.cs) respects [ResponseType](Wolfringo.Core/Messages/Responses/ResponseTypeAttribute.cs) attribute on the message type, and will ignore the generic type passed in with `SendAsync` method. If the attribute is missing, default client implementation will instruct the type resolver to use provided generic type. Client will attempt to cast the response to the provided generic type regardless of the actual response type, and might throw an exception if the cast is impossible.

## Further development
#### Known bugs and missing features
- Avatar setting is not supported, due to Wolf protocol not supporting it yet.
- Spam filter settings is not supported.

### Contributing
To ask questions or give ideas for Wolfringo, start a new [Discussion](https://github.com/TehGM/Wolfringo/discussions).

In case you want to report a bug or request a new feature, open a new [Issue](https://github.com/TehGM/Wolfringo/issues).

If you want to contribute a patch or update, fork repository, implement the change, and open a pull request.

## License
Copyright (c) 2020 TehGM 

Licensed under [MIT License](LICENSE).
