using Basyc.MessageBus.NetMQ.Shared.Cases;
using OneOf;

namespace Basyc.MessageBus.NetMQ.Shared;

public interface INetMqMessageWrapper
{
    OneOf<CheckInMessage, RequestCase, ResponseCase, EventCase, DeserializationFailureCase> ReadWrapperMessage(byte[] messageBytes);
    byte[] CreateWrapperMessage(object? messageData, string messageType, int sessionId, string traceId, string parentSpanId, MessageCase messageCase);
}
