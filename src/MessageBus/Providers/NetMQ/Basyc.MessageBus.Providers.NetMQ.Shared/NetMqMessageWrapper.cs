using Basyc.MessageBus.NetMQ.Shared.Cases;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;
using OneOf;

namespace Basyc.MessageBus.NetMQ.Shared;

public class NetMqMessageWrapper : INetMqMessageWrapper
{
    private static readonly string wrapperMessageType = TypedToSimpleConverter.ConvertTypeToSimple<ProtoMessageWrapper>();
    private readonly IObjectToByteSerailizer objectToByteSerializer;

    public NetMqMessageWrapper(IObjectToByteSerailizer byteSerializer)
    {
        objectToByteSerializer = byteSerializer;
    }

    public byte[] CreateWrapperMessage(object? messageData, string messageType, int sessionId, string traceId, string parentSpanId, MessageCase messageCase)
    {
        var messageBytes = messageData as byte[] ?? objectToByteSerializer.Serialize(messageData, messageType).Value();
        var wrapperMessageData = new ProtoMessageWrapper(sessionId, messageCase, messageType, messageBytes, traceId, parentSpanId);
        return objectToByteSerializer.Serialize(wrapperMessageData, wrapperMessageType);
    }

    public OneOf<CheckInMessage, RequestCase, ResponseCase, EventCase, DeserializationFailureCase> ReadWrapperMessage(byte[] messageBytes)
    {
        var wrapper = (ProtoMessageWrapper)objectToByteSerializer.Deserialize(messageBytes, wrapperMessageType).Value("Deserialization failed");
        switch (wrapper.MessageCase)
        {
            case MessageCase.CheckIn:
                try
                {
                    var checkIn = (CheckInMessage?)objectToByteSerializer.Deserialize(wrapper.MessageBytes, wrapper.MessageType);
                    if (checkIn is null)
                    {
                        return new DeserializationFailureCase(wrapper.SessionId,
                            wrapper.TraceId,
                            wrapper.ParentSpanId,
                            wrapper.MessageCase,
                            wrapper.MessageType,
                            null,
                            string.Empty);
                    }

                    return checkIn;
                }
                catch (Exception ex)
                {
                    return new DeserializationFailureCase(wrapper.SessionId,
                        wrapper.TraceId,
                        wrapper.ParentSpanId,
                        wrapper.MessageCase,
                        wrapper.MessageType,
                        ex,
                        $"{ex.Message}");
                }

            case MessageCase.Request:
                var requestCase = new RequestCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageType, wrapper.MessageBytes, false);
                return requestCase;
            case MessageCase.Response:
                var responseCase = new ResponseCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageBytes, wrapper.MessageType);
                return responseCase;
            case MessageCase.Event:
                var eventCase = new EventCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageType, wrapper.MessageBytes);
                return eventCase;
            default:
                throw new InvalidOperationException();
        }
    }
}
