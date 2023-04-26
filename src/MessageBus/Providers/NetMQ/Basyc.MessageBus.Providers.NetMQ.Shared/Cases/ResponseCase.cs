namespace Basyc.MessageBus.NetMQ.Shared.Cases;

#pragma warning disable CA1819 // Properties should not return arrays
public record ResponseCase(int SessionId, string TraceId, string ParentSpanId, byte[] ResponseBytes, string ResponseType)
     : CaseBase(SessionId, TraceId, ParentSpanId);
