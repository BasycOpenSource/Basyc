using Basyc.MessageBus.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client;

public interface ITypedMessageBusClient : IDisposable
{
	BusTask PublishAsync<TEvent>(RequestContext requestContext = default, CancellationToken cancellationToken = default)
		  where TEvent : class, IEvent, new();

	BusTask PublishAsync<TEvent>(TEvent eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		   where TEvent : notnull, IEvent;

	BusTask SendAsync<TCommand>(RequestContext requestContext = default, CancellationToken cancellationToken = default)
		 where TCommand : class, IMessage, new();
	BusTask SendAsync<TCommand>(TCommand commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		 where TCommand : notnull, IMessage;
	BusTask SendAsync(Type commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default);
	BusTask SendAsync(Type commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default);

	BusTask<TResponse> RequestAsync<TRequest, TResponse>(RequestContext requestContext = default, CancellationToken cancellationToken = default)
		 where TRequest : class, IMessage<TResponse>, new()
		 where TResponse : class;
	BusTask<object> RequestAsync(Type requestType, Type responseType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
	BusTask<object> RequestAsync(Type requestType, object requestData, Type responseType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
	BusTask<TResponse> RequestAsync<TRequest, TResponse>(TRequest requestData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		where TRequest : class, IMessage<TResponse>
		where TResponse : class;

	Task StartAsync(CancellationToken cancellationToken = default);
}