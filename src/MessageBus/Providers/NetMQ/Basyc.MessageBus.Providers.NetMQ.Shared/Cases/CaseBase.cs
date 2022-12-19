namespace Basyc.MessageBus.NetMQ.Shared.Cases
{
    public record CaseBase(int SessionId, string TraceId, string ParentSpanId);
}