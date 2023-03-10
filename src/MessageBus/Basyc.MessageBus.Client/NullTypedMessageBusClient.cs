using Basyc.MessageBus.Shared;

namespace Basyc.MessageBus.Client;

public class NullTypedMessageBusClient : ITypedMessageBusClient
{
	public void Dispose()
	{
	}

	public BusTask PublishAsync<TEvent>(RequestContext requestContext = default, CancellationToken cancellationToken = default) where TEvent : class, IEvent, new()
	{
		return BusTask.FromValue(requestContext.TraceId, new BusTaskCompleted()).ToBusTask();
	}

	public BusTask PublishAsync<TEvent>(TEvent eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		where TEvent : notnull, IEvent
	{
		return BusTask.FromValue(requestContext.TraceId, new BusTaskCompleted()).ToBusTask();
	}

	public BusTask SendAsync<TCommand>(RequestContext requestContext = default, CancellationToken cancellationToken = default) where TCommand : class, IMessage, new()
	{
		return BusTask.FromValue(requestContext.TraceId, new BusTaskCompleted()).ToBusTask();
	}

	public BusTask SendAsync<TCommand>(TCommand commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		where TCommand : notnull, IMessage
	{
		return BusTask.FromValue(requestContext.TraceId, new BusTaskCompleted()).ToBusTask();
	}

	public BusTask SendAsync(Type commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return BusTask.FromValue(requestContext.TraceId, new BusTaskCompleted()).ToBusTask();
	}

	public BusTask SendAsync(Type commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return BusTask.FromValue(requestContext.TraceId, new BusTaskCompleted()).ToBusTask();
	}

	public BusTask<TResponse> RequestAsync<TRequest, TResponse>(RequestContext requestContext = default, CancellationToken cancellationToken = default)
		where TRequest : class, IMessage<TResponse>, new() where TResponse : class
	{
		return BusTask<TResponse>.FromValue(requestContext.TraceId, default!);
	}

	public BusTask<object> RequestAsync(Type requestType, Type responseType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return BusTask<object>.FromValue(requestContext.TraceId, default!);
	}

	public BusTask<object> RequestAsync(Type requestType, object requestData, Type responseType, RequestContext requestContext = default,
		CancellationToken cancellationToken = default)
	{
		return BusTask<object>.FromValue(requestContext.TraceId, default!);
	}

	public BusTask<TResponse> RequestAsync<TRequest, TResponse>(TRequest requestData, RequestContext requestContext = default,
		CancellationToken cancellationToken = default) where TRequest : class, IMessage<TResponse> where TResponse : class
	{
		return BusTask<TResponse>.FromValue(requestContext.TraceId, default!);
	}

	public Task StartAsync(CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}
}
