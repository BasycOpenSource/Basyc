namespace Basyc.MessageBus.NetMQ.Shared.Cases;

public record ResponseCase(int SessionId, string TraceId, string ParentSpanId, byte[] ResponseBytes, string ResponseType)
     : CaseBase(SessionId, TraceId, ParentSpanId);
