using ProtoBuf;

namespace Basyc.Serialization.ProtobufNet.UnitTests;

#pragma warning disable CA1819 // Properties should not return arrays
//Supressing warning since this ctor is only used for serializers
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

[ProtoContract]
public class ParentWrapperMessage
{
    public ParentWrapperMessage(int sessionId, string messageType, byte[] messageData)
    {
        SessionId = sessionId;
        MessageType = messageType;
        MessageData = messageData;
    }

    protected ParentWrapperMessage()
    {
    }

    [ProtoMember(1)]
    public int SessionId { get; set; }

    [ProtoMember(3)]
    public string MessageType { get; set; }

    [ProtoMember(4)]
    public byte[] MessageData { get; set; }
}
