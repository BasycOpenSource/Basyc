using ProtoBuf;

namespace Basyc.MessageBus.HttpProxy.Shared.Http;
#pragma warning disable CS8618
#pragma warning disable CA1819 // Properties should not return arrays

[ProtoContract]
public class ResponseHttpDto
{
    public ResponseHttpDto(string traceId) : this(traceId, null, null)
    {
    }

    public ResponseHttpDto(string traceId, byte[]? responseData, string? responseType)
    {
        TraceId = traceId;
        ResponseBytes = responseData ?? Array.Empty<byte>();
        ResponseType = responseType;
    }

    protected ResponseHttpDto()
    {
    }

    [ProtoMember(5)] public string TraceId { get; }

    [ProtoMember(2)] public byte[]? ResponseBytes { get; set; }

    [ProtoMember(3)] public string? ResponseType { get; set; }
}
