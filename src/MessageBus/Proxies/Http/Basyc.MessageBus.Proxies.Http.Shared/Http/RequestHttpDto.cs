using ProtoBuf;

namespace Basyc.MessageBus.HttpProxy.Shared.Http;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CA1819 // Properties should not return arrays

[ProtoContract]
public class RequestHttpDto
{
    public RequestHttpDto(string messageType, bool hasResponse, byte[]? messageBytes = null, string? responseType = null)
    {
        MessageType = messageType;
        MessageBytes = messageBytes ?? Array.Empty<byte>();
        ResponseType = responseType;
        HasResponse = hasResponse;
    }

    //Needed for serialization
    protected RequestHttpDto()
    {
    }

    [ProtoMember(1)] public string MessageType { get; set; }

    [ProtoMember(2)] public byte[]? MessageBytes { get; set; }

    [ProtoMember(3)] public string? ResponseType { get; set; }

    [ProtoMember(4)] public bool HasResponse { get; set; }
}
