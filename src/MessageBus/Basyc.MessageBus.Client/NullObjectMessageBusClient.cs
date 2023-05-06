using Basyc.MessageBus.Client;
using Basyc.MessageBus.Shared;

namespace Microsoft.Extensions.DependencyInjection;

public class NullObjectMessageBusClient : IObjectMessageBusClient
{
    public void Dispose()
    {
    }

    public BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => BusTask.FromValue(requestContext.TraceId, default).ToBusTask();

    public BusTask PublishAsync(string eventType, object eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default) => BusTask.FromValue(requestContext.TraceId, default).ToBusTask();

    public BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => BusTask.FromValue(requestContext.TraceId, default).ToBusTask();

    public BusTask SendAsync(string commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default) => BusTask.FromValue(requestContext.TraceId, default).ToBusTask();

    public BusTask<object> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => BusTask<object>.FromValue(requestContext.TraceId, new object());

    public BusTask<object> RequestAsync(string requestType, object requestData, RequestContext requestContext = default, CancellationToken cancellationToken = default) => BusTask<object>.FromValue(requestContext.TraceId, new object());

    public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
