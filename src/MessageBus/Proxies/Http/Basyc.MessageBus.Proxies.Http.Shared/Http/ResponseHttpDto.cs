using ProtoBuf;

namespace Basyc.MessageBus.HttpProxy.Shared.Http;

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

#pragma warning disable CS8618
    protected ResponseHttpDto()
#pragma warning restore CS8618
    {
    }

    [ProtoMember(5)] public string TraceId { get; }

    [ProtoMember(2)] public byte[]? ResponseBytes { get; set; }

    [ProtoMember(3)] public string? ResponseType { get; set; }
}
