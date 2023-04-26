namespace Basyc.MessageBus.NetMQ.Shared.Cases;

#pragma warning disable CA1819 // Properties should not return arrays
public record EventCase(int SessionId, string TraceId, string ParentSpanId, string EventType, byte[] EventBytes)
    : CaseBase(SessionId, TraceId, ParentSpanId);
