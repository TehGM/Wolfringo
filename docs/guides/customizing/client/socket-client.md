---
uid: Guides.Customizing.Client.SocketClient
title: Customizing Wolfringo - Socket.IO Client
---

# Socket.IO Client
@TehGM.Wolfringo.WolfClient handles all Wolfringo-high level connectivity concerns. But under the hood, is uses a different service - @TehGM.Wolfringo.Socket.ISocketClient. This service is designed to handle all low-level Socket.IO (transport protocol used by WOLF Protocol v3) that @TehGM.Wolfringo.WolfClient requires.

@TehGM.Wolfringo.Socket.ISocketClient is responsible for maintaining WebSocket connection to with WOLF Server and responding to ping packets, as well as sending and receiving carefully serialized/deserialized low-level [SocketMessages](xref:TehGM.Wolfringo.Socket.SocketMessage) that are compatible with Socket.IO protocol.

You can check the default implementation on GitHub: [SocketClient.cs](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Core/Socket/SocketClient.cs).

## Custom Socket Client
The default implementation of Socket Client is designed to work with Wolfringo specifically, so it might not support all Socket.IO protocol features. If you need to extend or replace the socket client code, you can create a new class that implements @TehGM.Wolfringo.Socket.ISocketClient interface. This interface specifies a few members:  

Properties:
- @TehGM.Wolfringo.Socket.ISocketClient.IsConnected - indicates whether the client is currently connected.

Events:
- @TehGM.Wolfringo.Socket.ISocketClient.Connected - when the client establishes a connection with the server.
- @TehGM.Wolfringo.Socket.ISocketClient.Disconnected - when the client loses the connection with the server.
- @TehGM.Wolfringo.Socket.ISocketClient.MessageReceived - when the client receives a message from the server.
- @TehGM.Wolfringo.Socket.ISocketClient.MessageSent - when the client successfuly sends a message to the server.
- @TehGM.Wolfringo.Socket.ISocketClient.ErrorRaised - when there has been an exception in connection handling loop.

Methods:
- @TehGM.Wolfringo.Socket.ISocketClient.ConnectAsync(System.Uri,System.Threading.CancellationToken) - connects to the server on a given URI.
- @TehGM.Wolfringo.Socket.ISocketClient.DisconnectAsync(System.Threading.CancellationToken) - disconnects from the server.
- <xref:TehGM.Wolfringo.Socket.ISocketClient.SendAsync(Newtonsoft.Json.Linq.JToken,System.Collections.Generic.IEnumerable{System.Byte[]},System.Threading.CancellationToken)> - sends a message to the server.

Once your custom class is finished, you need to register it with @TehGM.Wolfringo.WolfClientBuilder as explained in [Introduction](xref:Guides.Customizing.Intro).

> [!WARNING]
> Creating a custom socket client is a delicate task. @TehGM.Wolfringo.Socket.ISocketClient is the fundamental building block for entire Wolfringo library, and WebSocket networking code and connection loop is very error-prone.  
> For this reason, it's recommended to ***NOT*** create custom Socket Client and use Wolfringo's implementation, unless customization is really necessary.