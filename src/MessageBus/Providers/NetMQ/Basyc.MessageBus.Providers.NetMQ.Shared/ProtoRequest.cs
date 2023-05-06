using ProtoBuf;

namespace Basyc.MessageBus.NetMQ.Shared;

[ProtoContract]
public class ProtoRequest
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string? Name { get; set; }

    [ProtoMember(3)]
    public string? Address { get; set; }
}
