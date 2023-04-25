using Basyc.MessageBus.NetMQ.Shared.Cases;
using ProtoBuf;

namespace Basyc.MessageBus.NetMQ.Shared;

[ProtoContract]
public class ProtoMessageWrapper
{
    public ProtoMessageWrapper(int sessionId, MessageCase messageCase, string messageType, byte[] messageData, string traceId, string parentSpanId)
    {
        SessionId = sessionId;
        MessageCase = messageCase;
        MessageType = messageType;
        MessageBytes = messageData;
        TraceId = traceId;
        ParentSpanId = parentSpanId;
    }

    //Suppressing warning since this ctor is only used for serializers
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected ProtoMessageWrapper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    [ProtoMember(1)]
    public int SessionId { get; set; }

    [ProtoMember(2)]
    public MessageCase MessageCase { get; }

    [ProtoMember(3)]
    public string MessageType { get; set; }

    [ProtoMember(4)]
    public byte[] MessageBytes { get; set; }

    [ProtoMember(5)]
    public string TraceId { get; set; }

    [ProtoMember(6)]
    public string ParentSpanId { get; set; }
}
