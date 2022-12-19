using Basyc.Diagnostics.Producing.Shared;
using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client
{
	public class TypedFromByteMessageBusClient : ITypedMessageBusClient
	{
		private readonly IByteMessageBusClient byteMessageBusClient;
		private readonly IObjectToByteSerailizer objectToByteSerailizer;
		private readonly IDiagnosticsExporter diagnosticsProducer;

		public TypedFromByteMessageBusClient(IByteMessageBusClient byteMessageBusClient, IObjectToByteSerailizer objectToByteSerailizer, IDiagnosticsExporter diagnosticsProducer)
		{
			this.byteMessageBusClient = byteMessageBusClient;
			this.objectToByteSerailizer = objectToByteSerailizer;
			this.diagnosticsProducer = diagnosticsProducer;
		}

		public void Dispose()
		{
			if (byteMessageBusClient is IDisposable disposable)
				disposable.Dispose();
		}

		public BusTask PublishAsync<TEvent>(TEvent eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default) where TEvent : notnull, IEvent
		{
			return byteMessageBusClient.PublishAsync(TypedToSimpleConverter.ConvertTypeToSimple(typeof(TEvent)), requestContext, cancellationToken);
		}

		public BusTask<object> RequestAsync(Type requestType, Type responseType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			var requestTypeString = TypedToSimpleConverter.ConvertTypeToSimple(requestType);
			var responseTypeString = TypedToSimpleConverter.ConvertTypeToSimple(responseType);

			var ínnerBusTask = byteMessageBusClient.RequestAsync(requestTypeString, requestContext, cancellationToken);
			return ínnerBusTask.ContinueWith<object>(x =>
			{
				return objectToByteSerailizer.Deserialize(x.ResponseBytes, x.ResposneType);
			});
		}

		public BusTask<object> RequestAsync(Type requestType, object requestData, Type responseType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			using var requestActivityDisposer = diagnosticsProducer.StartActivity(requestContext.TraceId, requestContext.ParentSpanId, "Typed RequestAsync");

			var requestTypeString = TypedToSimpleConverter.ConvertTypeToSimple(requestType);
			var responseTypeString = TypedToSimpleConverter.ConvertTypeToSimple(responseType);
			var requestBytes = objectToByteSerailizer.Serialize(requestData, requestTypeString);

			var byteBusTask = byteMessageBusClient.RequestAsync(requestTypeString, requestBytes, requestContext, cancellationToken);
			var objectBusTask = BusTask<object>.FromBusTask(byteBusTask, byteResponse => objectToByteSerailizer.Deserialize(byteResponse.ResponseBytes, byteResponse.ResposneType));
			return objectBusTask;
		}

		public BusTask SendAsync<TCommand>(TCommand commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default) where TCommand : notnull, IMessage
		{
			var commandTypeString = TypedToSimpleConverter.ConvertTypeToSimple(typeof(TCommand));
			return byteMessageBusClient.SendAsync(commandTypeString, requestContext, cancellationToken);
		}

		public BusTask SendAsync(Type commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			var commandTypeString = TypedToSimpleConverter.ConvertTypeToSimple(commandType);
			var requestBytes = objectToByteSerailizer.Serialize(commandData, commandTypeString);
			return byteMessageBusClient.SendAsync(commandTypeString, requestBytes, requestContext, cancellationToken);
		}

		public BusTask SendAsync(Type commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			var commandTypeString = TypedToSimpleConverter.ConvertTypeToSimple(commandType);
			return byteMessageBusClient.SendAsync(commandTypeString, requestContext, cancellationToken);
		}

		public Task StartAsync(CancellationToken cancellationToken = default)
		{
			return byteMessageBusClient.StartAsync(cancellationToken);
		}

		BusTask ITypedMessageBusClient.PublishAsync<TEvent>(RequestContext requestContext, CancellationToken cancellationToken)
		{
			var eventTypeString = TypedToSimpleConverter.ConvertTypeToSimple(typeof(TEvent));
			return byteMessageBusClient.PublishAsync(eventTypeString, requestContext, cancellationToken);
		}

		BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(RequestContext requestContext, CancellationToken cancellationToken)
		{
			return RequestAsync(typeof(TRequest), typeof(TResponse), requestContext, cancellationToken).ContinueWith<TResponse>(x => (TResponse)x);

		}

		BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(TRequest requestData, RequestContext requestContext, CancellationToken cancellationToken)
		{
			return RequestAsync(typeof(TRequest), requestData, typeof(TResponse), requestContext, cancellationToken).ContinueWith<TResponse>(x => (TResponse)x);
		}

		BusTask ITypedMessageBusClient.SendAsync<TCommand>(RequestContext requestContext, CancellationToken cancellationToken)
		{
			return SendAsync(typeof(TCommand), cancellationToken);
		}
	}
}
