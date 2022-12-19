using Basyc.MessageBus.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client
{
	public interface IObjectMessageBusClient : IDisposable
	{
		BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
		BusTask PublishAsync(string eventType, object eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default);

		BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
		BusTask SendAsync(string commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default);

		BusTask<object> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
		BusTask<object> RequestAsync(string requestType, object requestData, RequestContext requestContext = default, CancellationToken cancellationToken = default);

		Task StartAsync(CancellationToken cancellationToken = default);
	}
}
