﻿using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.IO.Pipelines;
using System.Net;

namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

#pragma warning disable CA1003 // Use generic event handler instances

public class HubConnectionMock : HubConnection
{
    private readonly Pipe pipe;

    private readonly HubProtocolMock hubProtocolMock;

    public HubConnectionMock(Pipe pipe,
        IConnectionFactory connectionFactory,
        HubProtocolMock protocol,
        EndPoint endPoint,
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory,
        IRetryPolicy reconnectPolicy)
        : base(connectionFactory, protocol, endPoint, serviceProvider, loggerFactory, reconnectPolicy)
    {
        this.pipe = pipe;
        hubProtocolMock = protocol;
    }

    public event EventHandler<SendingCoreArgs>? SendingCore;

    public SendingCoreArgs? LastSendCoreCall { get; private set; }

    public override Task SendCoreAsync(string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        OnSendingCore(new(methodName, args, cancellationToken));
        return Task.CompletedTask;
    }

    public async Task ReceiveMessage(string messageName, object?[] arguments)
    {
        hubProtocolMock.AddReceivingMessage(new HubProtocolMockMessage(messageName, arguments));
        await pipe.Writer.WriteAsync(new byte[] { 0 });
        await Task.Delay(100); //Give SignalR time to process
    }

    private void OnSendingCore(SendingCoreArgs args)
    {
        LastSendCoreCall = args;
        SendingCore?.Invoke(this, args);
    }
}
