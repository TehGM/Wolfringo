# Wolfringo's SocketIO client implementation
This folder contains custom SocketIO client implementation. Feel free to use it in your own project, following [Wolfringo's MIT licensing](/LICENSE).

This implementation was created because most of SocketIO libraries are obsolete, don't work with .NET Core, or are bugged, WIP, or have other issues. Because of this situation, I decided to write my own implementation that I'll have full control and knowledge of, and that works fine with the library.

> Please note that the client implementation is designed specifically for use with WOLF/Palringo. As such, it may not support all SocketIO features, and some behaviours may be WOLF-specific.

## Usage
```csharp
// init socket client
ISocketClient client = new SocketClient();

// listen to events
client.MessageReceived += OnClientMessageReceived;
client.MessageSent += OnClientMessageSent;
client.Connected += OnClientConnected;
client.Disconnected += OnClientDisconnected;
client.ErrorRaised += OnClientError;

// connect
await client.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);

// send a message
IEnumerable<byte[]> binaryMessages = ...
JObject jsonPayload = ...
await client.SendAsync("message send", jsonPayload, binaryMessages, cancellationToken).ConfigureAwait(false);
```

See [WolfClient.cs](/Wolfringo.Core/WolfClient.cs) for a working implementation example.

#### Events
This SocketIO client implementation does not support logging. Instead, it'll communicate using exceptions (for errors occured in methods when invoked externally) and events. Listen to these events to determine what happened and optionally log. Default [WolfClient](/Wolfringo.Core/WolfClient.cs) implementation does this automatically using provided ILogger.

`ErrorRaised` event will not be invoked for task cancellations (they're considered to be a normal disconnection), and server abruptly ending the connection (instead of raising error, the reason will be mentioned in Disconnected event).

#### Session ID and ping loop
The default [SocketClient](SocketClient.cs) implementation will automatically handle SID message when connection is established, and then start a background ping loop. No manual actions are required.

Ping loop will automatically exit when connection is dropped.

You can access parsed SID object using `Session` property once the connection has been established.

#### Encoding
By default, client uses UTF-8 encoding when sending and receiving messages. To change that, simply set `MessagesEncoding` property.

## License
Copyright (c) 2020 TehGM 

Licensed under [MIT License](/LICENSE).