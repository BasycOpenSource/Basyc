using Basyc.MessageBus.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client
{
	public interface IByteMessageBusClient
	{
		BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
		BusTask PublishAsync(string eventType, byte[] eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default);

		BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
		BusTask SendAsync(string commandType, byte[] commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default);

		BusTask<ByteResponse> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default);
		BusTask<ByteResponse> RequestAsync(string requestType, byte[] requestData, RequestContext requestContext = default, CancellationToken cancellationToken = default);

		Task StartAsync(CancellationToken cancellationToken = default);
	}
}
