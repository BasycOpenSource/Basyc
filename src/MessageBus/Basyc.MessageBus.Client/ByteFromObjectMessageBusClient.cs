using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;

namespace Basyc.MessageBus.Client;

public class ByteFromObjectMessageBusClient : IByteMessageBusClient
{
    private readonly IObjectMessageBusClient objectMessageBusClient;
    private readonly IObjectToByteSerailizer byteSerailizer;

    public ByteFromObjectMessageBusClient(IObjectMessageBusClient objectMessageBusClient, IObjectToByteSerailizer byteSerailizer)
    {
        this.objectMessageBusClient = objectMessageBusClient;
        this.byteSerailizer = byteSerailizer;
    }

    public BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => objectMessageBusClient.PublishAsync(eventType, cancellationToken, cancellationToken: cancellationToken);

    public BusTask PublishAsync(string eventType, byte[] eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default) => objectMessageBusClient.PublishAsync(eventType, eventData, requestContext, cancellationToken);

    public BusTask<ByteResponse> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
    {
        var innerBusTask = objectMessageBusClient.RequestAsync(requestType, cancellationToken, cancellationToken: cancellationToken);
        return innerBusTask.ContinueWith<ByteResponse>(x => new ByteResponse((byte[])x, "unknown"));
    }

    public BusTask<ByteResponse> RequestAsync(string requestType, byte[] requestData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
    {
        var innerBusTask = objectMessageBusClient.RequestAsync(requestType, requestData, requestContext, cancellationToken);
        return innerBusTask.ContinueWith<ByteResponse>(nestedValue =>
        {
            if (nestedValue is byte[] bytes)
                return new ByteResponse(bytes, "unknown");
            else
                return new ErrorMessage("Does not know how to serialize");
        });
    }

    public BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default) => objectMessageBusClient.SendAsync(commandType, cancellationToken, cancellationToken: cancellationToken);

    public BusTask SendAsync(string commandType, byte[] commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default) => objectMessageBusClient.SendAsync(commandType, commandData, requestContext, cancellationToken);

    public Task StartAsync(CancellationToken cancellationToken = default) => objectMessageBusClient.StartAsync(cancellationToken);
}
