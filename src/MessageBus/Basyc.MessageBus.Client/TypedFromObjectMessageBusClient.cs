﻿using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.MessageBus.Shared;
using Basyc.Serializaton.Abstraction;
using OneOf;

namespace Basyc.MessageBus.Client;

public sealed class TypedFromObjectMessageBusClient : ITypedMessageBusClient
{
    private readonly IDiagnosticsExporter diagnosticExporter;
    private readonly IObjectMessageBusClient objectBusClient;

    public TypedFromObjectMessageBusClient(IObjectMessageBusClient messageBusClient, IDiagnosticsExporter diagnosticExporter)
    {
        objectBusClient = messageBusClient;
        this.diagnosticExporter = diagnosticExporter;
    }

    public Task StartAsync(CancellationToken cancellationToken = default) => objectBusClient.StartAsync(cancellationToken);

    void IDisposable.Dispose() => objectBusClient.Dispose();

    BusTask ITypedMessageBusClient.PublishAsync<TEvent>(RequestContext requestContext, CancellationToken cancellationToken) => objectBusClient.PublishAsync(TypedToSimpleConverter.ConvertTypeToSimple<TEvent>(), cancellationToken, cancellationToken: cancellationToken);

    BusTask ITypedMessageBusClient.PublishAsync<TEvent>(TEvent eventData, RequestContext requestContext, CancellationToken cancellationToken) => objectBusClient.PublishAsync(TypedToSimpleConverter.ConvertTypeToSimple<TEvent>(), eventData, requestContext, cancellationToken);

    BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(RequestContext requestContext, CancellationToken cancellationToken)
    {
        var nestedBusTask = objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple<TRequest>(), cancellationToken, cancellationToken: cancellationToken);
        return BusTask<TResponse>.FromBusTask(nestedBusTask, x => (TResponse)x);
    }

    BusTask<object> ITypedMessageBusClient.RequestAsync(Type requestType, Type responseType, RequestContext requestContext, CancellationToken cancellationToken) => objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple(requestType), cancellationToken, cancellationToken: cancellationToken);

    BusTask<object> ITypedMessageBusClient.RequestAsync(Type requestType,
        object requestData,
        Type responseType,
        RequestContext requestContext,
        CancellationToken cancellationToken) => objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple(requestType), requestData, requestContext, cancellationToken);

    BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(TRequest requestData, RequestContext requestContext, CancellationToken cancellationToken)
    {
        var innerBusTask = objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple<TRequest>(), requestData, requestContext, cancellationToken);
        return innerBusTask.ContinueWith(x => (OneOf<TResponse, ErrorMessage>)x);
    }

    BusTask ITypedMessageBusClient.SendAsync<TCommand>(RequestContext requestContext, CancellationToken cancellationToken) => objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple<TCommand>(), cancellationToken, cancellationToken: cancellationToken);

    BusTask ITypedMessageBusClient.SendAsync<TCommand>(TCommand command, RequestContext requestContext, CancellationToken cancellationToken) => objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple<TCommand>(), command, requestContext, cancellationToken);

    BusTask ITypedMessageBusClient.SendAsync(Type commandType, object command, RequestContext requestContext, CancellationToken cancellationToken) => objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple(commandType), command, requestContext, cancellationToken);

    BusTask ITypedMessageBusClient.SendAsync(Type commandType, RequestContext requestContext, CancellationToken cancellationToken) => objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple(commandType), cancellationToken, cancellationToken: cancellationToken);
}
