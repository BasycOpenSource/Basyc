using Basyc.MessageBus.NetMQ.Shared.Cases;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;
using OneOf;

namespace Basyc.MessageBus.NetMQ.Shared;

public class NetMQMessageWrapper : INetMQMessageWrapper
{
	private readonly IObjectToByteSerailizer objectToByteSerializer;
	private static readonly string wrapperMessageType = TypedToSimpleConverter.ConvertTypeToSimple<ProtoMessageWrapper>();

	public NetMQMessageWrapper(IObjectToByteSerailizer byteSerailizer)
	{
		this.objectToByteSerializer = byteSerailizer;
	}

	public byte[] CreateWrapperMessage(object? messageData, string messageType, int sessionId, string traceId, string requesterSpanId, MessageCase messageCase)
	{
		byte[] messageBytes = messageData is byte[] bytes ? bytes : objectToByteSerializer.Serialize(messageData, messageType);

		if (messageBytes is null)
			throw new Exception();

		var wrapperMessageData = new ProtoMessageWrapper(sessionId, messageCase, messageType, messageBytes, traceId, requesterSpanId);
		return objectToByteSerializer.Serialize(wrapperMessageData, wrapperMessageType);
	}

	public OneOf<CheckInMessage, RequestCase, ResponseCase, EventCase, DeserializationFailureCase> ReadWrapperMessage(byte[] wrapperBytes)
	{
		ProtoMessageWrapper? wrapper = (ProtoMessageWrapper?)objectToByteSerializer.Deserialize(wrapperBytes, wrapperMessageType);
		if (wrapper is null)
			throw new Exception("Deserialization failed");

		switch (wrapper.MessageCase)
		{
			case MessageCase.CheckIn:
				try
				{
					var checkIn = (CheckInMessage?)objectToByteSerializer.Deserialize(wrapper.MessageBytes, wrapper.MessageType);
					if (checkIn is null)
						return new DeserializationFailureCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageCase, wrapper.MessageType, null, string.Empty);
					return checkIn;
				}
				catch (Exception ex)
				{
					return new DeserializationFailureCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageCase, wrapper.MessageType, ex, $"{ex.Message}");
				}

			case MessageCase.Request:
				RequestCase requestCase = new RequestCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageType, wrapper.MessageBytes, false);
				return requestCase;
			case MessageCase.Response:
				ResponseCase responseCase = new ResponseCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageBytes, wrapper.MessageType);
				return responseCase;
			case MessageCase.Event:
				var eventCase = new EventCase(wrapper.SessionId, wrapper.TraceId, wrapper.ParentSpanId, wrapper.MessageType, wrapper.MessageBytes);
				return eventCase;
			default:
				throw new Exception();
		}
	}
}