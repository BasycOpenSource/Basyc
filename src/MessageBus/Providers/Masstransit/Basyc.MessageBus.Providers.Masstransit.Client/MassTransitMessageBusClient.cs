using Basyc.MessageBus.Shared;
using MassTransit;
using OneOf;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client.MasstTransit
{
	public class MassTransitMessageBusClient : ITypedMessageBusClient
	{
		private readonly IBusControl massTransitBus;

		public MassTransitMessageBusClient(IBusControl massTransitBus)
		{
			this.massTransitBus = massTransitBus;
		}

		BusTask ITypedMessageBusClient.PublishAsync<TEvent>(RequestContext requestContext, CancellationToken cancellationToken)
		{
			var publishTask = massTransitBus.Publish<TEvent>(cancellationToken);
			return BusTask.FromTask("-1", publishTask);
		}

		BusTask ITypedMessageBusClient.PublishAsync<TEvent>(TEvent data, RequestContext requestContext, CancellationToken cancellationToken)
		{
			var publishTask = massTransitBus.Publish(data, cancellationToken);
			return BusTask.FromTask("-1", publishTask);

		}

		BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(RequestContext requestContext, CancellationToken cancellationToken)
		{
			var requestTask = massTransitBus.Request<TRequest, TResponse>(cancellationToken);
			return BusTask<TResponse>.FromTask<Response<TResponse>>("-1", requestTask, x => (OneOf<TResponse, ErrorMessage>)x.Message);
		}

		BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(TRequest request, RequestContext requestContext, CancellationToken cancellationToken)
		{
			var requestTask = massTransitBus.Request<TRequest, TResponse>(request, cancellationToken);
			return BusTask<TResponse>.FromTask<Response<TResponse>>("-1", requestTask, x => (OneOf<TResponse, ErrorMessage>)x.Message);
		}

		BusTask<object> ITypedMessageBusClient.RequestAsync(Type requestType, Type responseType, RequestContext requestContext, CancellationToken cancellationToken)
		{
			var bus = (ITypedMessageBusClient)this;
			return bus.RequestAsync(requestType, Activator.CreateInstance(requestType), responseType, requestContext, cancellationToken);
		}

		public BusTask<object> RequestAsync(Type requestType, object request, Type responseType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			var conType = typeof(SendContext<>).MakeGenericType(requestType);
			var actionType = typeof(Action<>).MakeGenericType(conType);
			var methodParameterTypes = new Type[] { typeof(IBus), requestType, typeof(CancellationToken), typeof(RequestTimeout), actionType };
			//MassTransit does not have Request variant that accepts type as parameter (not type parameter)
			//MethodInfo requestMethodInfo = typeof(MassTransit.RequestExtensions).GetMethod(nameof(MassTransit.RequestExtensions.Request), BindingFlags.Public | BindingFlags.Static, null, methodParameterTypes, null);
			MethodInfo requestMethodInfo = typeof(RequestExtensions).GetMethods().Where(x => x.Name == nameof(RequestExtensions.Request)).Skip(2).First();
			var parameters = requestMethodInfo.GetParameters();
			MethodInfo genericMethod = requestMethodInfo.MakeGenericMethod(requestType, responseType);

			//var busResponse = (Response<object>)await InvokeAsync(genericMethod, null, new object[] { massTransitBus, request, cancellationToken, default(RequestTimeout), null });
			//return busResponse.Message;

			var busResponse = InvokeAsync(genericMethod, null, new object[] { massTransitBus, request, cancellationToken, default(RequestTimeout), null });
			var busTask = BusTask<object>.FromTask("-1", busResponse);
			return busTask;
		}

		public BusTask SendAsync(Type requestType, object request, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			//await _massTransitBus.Publish(request, requestType, cancellationToken); //Wont get response
			//await _massTransitBus.Send(request, requestType, cancellationToken); //Does not work

			//Command can return response, but should not query data, returning command completion status is allowed
			return RequestAsync(requestType, request, typeof(VoidCommandResult), requestContext, cancellationToken).ToBusTask();
		}

		public BusTask SendAsync(Type requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			var request = Activator.CreateInstance(requestType);
			//await _massTransitBus.Publish(request, requestType, cancellationToken);
			return SendAsync(requestType, request, requestContext, cancellationToken);
		}

		BusTask ITypedMessageBusClient.SendAsync<TRequest>(RequestContext requestContext, CancellationToken cancellationToken)
		{
			//await _massTransitBus.Publish<TRequest>(cancellationToken);
			return SendAsync(typeof(TRequest), requestContext, cancellationToken);
		}

		BusTask ITypedMessageBusClient.SendAsync<TRequest>(TRequest request, RequestContext requestContext, CancellationToken cancellationToken)
		{
			//await _massTransitBus.Publish(request, cancellationToken);
			return SendAsync(typeof(TRequest), request, requestContext, cancellationToken);
		}

		private static async Task<object> InvokeAsync(MethodInfo masstransitRequestMethod, object obj, params object[] parameters)
		{
			var task = (Task)masstransitRequestMethod.Invoke(obj, parameters);
			await task.ConfigureAwait(false);
			var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));
			return resultProperty.GetValue(task);
		}

		public void Dispose()
		{
			try
			{
				massTransitBus.Start(new TimeSpan(0, 0, 30));
			}
			catch (Exception ex)
			{
				throw new Exception("MassTransit timeout", ex);
			}
		}

		public Task StartAsync(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}