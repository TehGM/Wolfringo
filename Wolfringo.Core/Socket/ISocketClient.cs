﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    public interface ISocketClient
    {
        bool IsConnected { get; }

        event EventHandler Connected;
        event EventHandler<SocketClosedEventArgs> Disconnected;
        event EventHandler<SocketMessageEventArgs> MessageReceived;
        event EventHandler<SocketMessageEventArgs> MessageSent;
        event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        Task ConnectAsync(Uri url, CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        Task<uint> SendAsync(JToken payload, IEnumerable<byte[]> binaryMessages, CancellationToken cancellationToken = default);
    }
}
