using Basyc.Diagnostics.Shared;
using Basyc.Extensions.SignalR.Client;
using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Client.SignalR.Sessions;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;
using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using OneOf;
using System.Diagnostics;

namespace Basyc.MessageBus.HttpProxy.Client.Http;

public class SignalRProxyObjectMessageBusClient : IObjectMessageBusClient
{
    private readonly IObjectToByteSerailizer byteSerializer;
    private readonly IStrongTypedHubConnectionPusherAndReceiver<IMethodsClientCanCall, IClientMethodsServerCanCall> hubConnection;
    private readonly SignalRSessionManager sessionManager;

    public SignalRProxyObjectMessageBusClient(IOptions<SignalROptions> options,
        IObjectToByteSerailizer byteSerializer,
        ISharedRequestIdCounter requestIdCounter)
    {
        sessionManager = new SignalRSessionManager(requestIdCounter);
        hubConnection = new HubConnectionBuilder()
            .WithUrl(options.Value.SignalRServerUri + options.Value.ProxyClientHubPattern)
            .WithAutomaticReconnect()
            .BuildStrongTyped<IMethodsClientCanCall, IClientMethodsServerCanCall>(sessionManager);
        this.byteSerializer = byteSerializer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await sessionManager.Start();
        await hubConnection.StartAsync(cancellationToken);
    }

#pragma warning disable CA2012
    public void Dispose() => hubConnection.DisposeAsync().GetAwaiter().GetResult();
#pragma warning restore CA2012

    public BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => CreateAndStartBusTask(eventType, null, requestContext, cancellationToken).ToBusTask();

    public BusTask PublishAsync(string eventType, object eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default) => CreateAndStartBusTask(eventType, eventData, requestContext, cancellationToken).ToBusTask();

    public BusTask<object> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => BusTask<object>.FromBusTask(CreateAndStartBusTask(requestType, null, requestContext, cancellationToken), x => x!);

    public BusTask<object> RequestAsync(string requestType,
        object requestData,
        RequestContext requestContext = default,
        CancellationToken cancellationToken = default) => BusTask<object>.FromBusTask(CreateAndStartBusTask(requestType, requestData, requestContext, cancellationToken), x => x!);

    public BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => CreateAndStartBusTask(commandType, null, requestContext, cancellationToken).ToBusTask();

    public BusTask SendAsync(string commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default) => CreateAndStartBusTask(commandType, commandData, requestContext, cancellationToken).ToBusTask();

    private BusTask<object?> CreateAndStartBusTask(string requestType,
        object? requestData = null,
        RequestContext requestContext = default,
        CancellationToken cancellationToken = default)
    {
        var createAndStartBusTaskActivity = DiagnosticHelper.Start("SignalRProxyObjectMessageBusClient.CreateAndStartBusTask",
            requestContext.TraceId,
            requestContext.ParentSpanId);
        var session = sessionManager.StartSession(requestContext.TraceId);
        var waitingForTaskRunActivity = DiagnosticHelper.Start("Waiting for Task.Run");
        var requestTask = Task.Run(async () =>
                await BustaskMethod(requestType, requestData, requestContext, createAndStartBusTaskActivity, session, waitingForTaskRunActivity),
            cancellationToken);
        return BusTask<object?>.FromTask(session.TraceId, requestTask);
    }

    private async Task<OneOf<object?, ErrorMessage>> BustaskMethod(string requestType,
        object? requestData,
        RequestContext requestContext,
        ActivityDisposer createAndStartBusTaskActivity,
        SignalRSession session,
        ActivityDisposer waitingForTaskRunActivity)
    {
        waitingForTaskRunActivity.Stop();
        var busTaskActivity = DiagnosticHelper.Start("BustaskMethod");
#pragma warning disable CA2000 // Dispose objects before losing scope
        using var seriActivity = new Activity("Serializating request").Start();
#pragma warning restore CA2000 // Dispose objects before losing scope

        if (byteSerializer.TrySerialize(requestData, requestType, out var requestDataBytes, out var error) is false)
        {
            seriActivity.Stop();
            return new ErrorMessage("Failed to serailize request");
        }

        seriActivity.Stop();
        var signalRActivity = DiagnosticHelper.Start("SignalR Sending request");

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await hubConnection.Call.Request(new RequestSignalRDto(requestType, true, requestDataBytes, RequestContext: requestContext));
        }
        catch (Exception ex)
        {
            signalRActivity.Stop();
            return new ErrorMessage("Failed while requesting. " + ex.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        signalRActivity.Stop();

        var waitingForResult = DiagnosticHelper.Start("Waiting for result");
        var sessionRsult = await session.WaitForCompletion();
        waitingForResult.Stop();

        return sessionRsult.Match<OneOf<object?, ErrorMessage>>(resultDto =>
            {
                if (resultDto.HasResponse)
                {
                    var deseriActivity = DiagnosticHelper.Start("SignalR Request");

                    if (byteSerializer.TryDeserialize(resultDto.ResponseData!, resultDto.ResponseType!, out var deserializedResult, out var error) is false)
                    {
                        busTaskActivity.Stop();
                        createAndStartBusTaskActivity.Stop();
                        deseriActivity.Stop();
                        return new ErrorMessage("Failed to serailize response");
                    }

                    deseriActivity.Stop();
                    busTaskActivity.Stop();
                    createAndStartBusTaskActivity.Stop();
                    return deserializedResult;
                }

                busTaskActivity.Stop();
                createAndStartBusTaskActivity.Stop();
                return null!;
            },
            error =>
            {
                busTaskActivity.Stop();
                createAndStartBusTaskActivity.Stop();
                return error;
            });
    }
}
