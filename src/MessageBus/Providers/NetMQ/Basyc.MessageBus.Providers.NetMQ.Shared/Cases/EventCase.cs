namespace Basyc.MessageBus.NetMQ.Shared.Cases;

public record EventCase(int SessionId, string TraceId, string ParentSpanId, string EventType, byte[] EventBytes)
    : CaseBase(SessionId, TraceId, ParentSpanId);
