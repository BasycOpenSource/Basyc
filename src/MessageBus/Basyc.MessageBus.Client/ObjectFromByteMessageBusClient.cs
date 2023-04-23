using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;
using Throw;

namespace Basyc.MessageBus.Client;

public class ObjectFromByteMessageBusClient : IObjectMessageBusClient
{
    private readonly IByteMessageBusClient byteMessageBusClient;
    private readonly IObjectToByteSerailizer objectToByteSerailizer;

    public ObjectFromByteMessageBusClient(IByteMessageBusClient byteMessageBusClient, IObjectToByteSerailizer objectToByteSerailizer)
    {
        this.byteMessageBusClient = byteMessageBusClient;
        this.objectToByteSerailizer = objectToByteSerailizer;
    }

    public void Dispose()
    {
        if (byteMessageBusClient is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => byteMessageBusClient.PublishAsync(eventType, requestContext, cancellationToken);

    public BusTask PublishAsync(string eventType, object eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
    {
        var eventBytes = objectToByteSerailizer.Serialize(eventData, eventType);
        return byteMessageBusClient.PublishAsync(eventType, eventBytes, requestContext, cancellationToken);
    }

    public BusTask<object> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
    {
        var innerBusTask = byteMessageBusClient.RequestAsync(requestType, requestContext, cancellationToken);
        var busTask = innerBusTask.ContinueWith<object>(x =>
        {
            var deseriResult = objectToByteSerailizer.Deserialize(x.ResponseBytes, x.ResposneType);
            deseriResult.ThrowIfNull();
            return deseriResult;
        });
        return busTask;
    }

    public BusTask<object> RequestAsync(string requestType, object requestData, RequestContext requestContext = default,
        CancellationToken cancellationToken = default)
    {
        var requestBytes = objectToByteSerailizer.Serialize(requestData, requestType);
        var innerBusTask = byteMessageBusClient.RequestAsync(requestType, requestBytes, requestContext, cancellationToken);
        var busTask = BusTask<object>.FromBusTask(innerBusTask, byteResponse =>
        {
            var deseriliazed = objectToByteSerailizer.Deserialize(byteResponse.ResponseBytes, byteResponse.ResposneType);
            deseriliazed.ThrowIfNull();
            return deseriliazed;
        });
        return busTask;
    }

    public BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => byteMessageBusClient.SendAsync(commandType, requestContext, cancellationToken);

    public BusTask SendAsync(string commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
    {
        var requestBytes = objectToByteSerailizer.Serialize(commandData, commandType);
        return byteMessageBusClient.SendAsync(commandType, requestBytes, requestContext, cancellationToken);
    }

    public Task StartAsync(CancellationToken cancellationToken = default) => byteMessageBusClient.StartAsync(cancellationToken);
}
