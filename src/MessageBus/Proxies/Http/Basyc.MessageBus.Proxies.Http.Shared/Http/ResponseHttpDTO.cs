using ProtoBuf;
using System;

namespace Basyc.MessageBus.HttpProxy.Shared.Http;

[ProtoContract]
public class ResponseHttpDTO
{
    protected ResponseHttpDTO()
    {

    }

    public ResponseHttpDTO(string traceId) : this(traceId, null, null)
    {
    }

    public ResponseHttpDTO(string traceId, byte[]? responseData, string? responseType)
    {
        TraceId = traceId;
        ResponseBytes = responseData ?? Array.Empty<byte>();
        ResponseType = responseType;
    }

    [ProtoMember(5)]
    public string TraceId { get; }
    [ProtoMember(2)]
    public byte[]? ResponseBytes { get; set; }
    [ProtoMember(3)]
    public string? ResponseType { get; set; }
}
