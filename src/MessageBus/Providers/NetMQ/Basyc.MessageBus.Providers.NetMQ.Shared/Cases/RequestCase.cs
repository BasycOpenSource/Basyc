namespace Basyc.MessageBus.NetMQ.Shared.Cases;

#pragma warning disable CA1819 // Properties should not return arrays
public record RequestCase(int SessionId, string TraceId, string ParentSpanId, string RequestType, byte[] RequestBytes, bool ExpectsResponse)
     : CaseBase(SessionId, TraceId, ParentSpanId);
