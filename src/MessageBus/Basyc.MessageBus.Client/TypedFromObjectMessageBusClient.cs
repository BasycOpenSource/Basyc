using Basyc.Diagnostics.Producing.Shared;
using Basyc.MessageBus.Shared;
using Basyc.Serializaton.Abstraction;
using OneOf;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client;

public sealed class TypedFromObjectMessageBusClient : ITypedMessageBusClient
{
	private readonly IObjectMessageBusClient objectBusClient;
	private readonly IDiagnosticsExporter diagnosticExporter;

	public TypedFromObjectMessageBusClient(IObjectMessageBusClient messageBusClient, IDiagnosticsExporter diagnosticExporter)
	{
		this.objectBusClient = messageBusClient;
		this.diagnosticExporter = diagnosticExporter;
	}

	public Task StartAsync(CancellationToken cancellationToken = default)
	{
		return objectBusClient.StartAsync(cancellationToken);
	}

	void IDisposable.Dispose()
	{
		objectBusClient.Dispose();
	}

	BusTask ITypedMessageBusClient.PublishAsync<TEvent>(RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.PublishAsync(TypedToSimpleConverter.ConvertTypeToSimple<TEvent>(), cancellationToken);
	}

	BusTask ITypedMessageBusClient.PublishAsync<TEvent>(TEvent eventData, RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.PublishAsync(TypedToSimpleConverter.ConvertTypeToSimple<TEvent>(), eventData, requestContext, cancellationToken);
	}

	BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(RequestContext requestContext, CancellationToken cancellationToken)
	{
		var nestedBusTask = objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple<TRequest>(), cancellationToken);
		return BusTask<TResponse>.FromBusTask(nestedBusTask, x => (TResponse)x);
	}

	BusTask<object> ITypedMessageBusClient.RequestAsync(Type requestType, Type responseType, RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple(requestType), cancellationToken);
	}

	BusTask<object> ITypedMessageBusClient.RequestAsync(Type requestType, object requestData, Type responseType, RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple(requestType), requestData, requestContext, cancellationToken);
	}

	BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(TRequest requestData, RequestContext requestContext, CancellationToken cancellationToken)
	{
		var innerBusTask = objectBusClient.RequestAsync(TypedToSimpleConverter.ConvertTypeToSimple<TRequest>(), requestData, requestContext, cancellationToken);
		return innerBusTask.ContinueWith(x => (OneOf<TResponse, ErrorMessage>)x);

	}

	BusTask ITypedMessageBusClient.SendAsync<TCommand>(RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple<TCommand>(), cancellationToken);
	}

	BusTask ITypedMessageBusClient.SendAsync<TCommand>(TCommand command, RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple<TCommand>(), command, requestContext, cancellationToken);
	}

	BusTask ITypedMessageBusClient.SendAsync(Type commandType, object command, RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple(commandType), command, requestContext, cancellationToken);
	}

	BusTask ITypedMessageBusClient.SendAsync(Type commandType, RequestContext requestContext, CancellationToken cancellationToken)
	{
		return objectBusClient.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple(commandType), cancellationToken);
	}
}
