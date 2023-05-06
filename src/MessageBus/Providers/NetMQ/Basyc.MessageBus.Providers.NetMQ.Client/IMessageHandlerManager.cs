using OneOf;

namespace Basyc.MessageBus.Client.NetMQ;

public interface IMessageHandlerManager
{
    /// <summary>
    /// return response object or <see cref="MessageBus.NetMQ.Shared.VoidResult"/>.
    /// </summary>
    Task<OneOf<object, Exception>> ConsumeMessage(string messageType, object? messageData, string traceId, string parentId, CancellationToken cancellationToken);

    string[] GetConsumableMessageTypes();
}
